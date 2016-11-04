using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Diagnostics;

using CSRunner.Base;

using System.Linq;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Xml;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.Emit;

//http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b
namespace CSRunner.Base
{
    public class CSharpCompiler
    {
        static IResXCompiler _resXCompiler;
        static string _runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
        static CSharpParseOptions _parserOptions;

        static CSharpCompiler()
        {
            _runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
            _parserOptions = new CSharpParseOptions();//.WithLanguageVersion(LanguageVersion.CSharp6)
        }

        static IResXCompiler ResXCompiler
        {
            get
            {
                if (_resXCompiler == null)
                {
                    _resXCompiler = Activator.CreateInstance(Config.ResXCompiler) as IResXCompiler;
                }
                return _resXCompiler;
            }
        }

        public Action<string, object[]> ErrorHandler
        {
            get;
            set;
        }

        public bool Debug
        {
            get;
            private set;
        }

        public string AssemblyName
        {
            get;
            private set;
        }

        public List<string> Files
        {
            get;
            private set;
        }

        public List<Resource> Resources
        {
            get;
            private set;
        }

        public List<string> ReferenceAssemblies
        {
            get;
            private set;
        }

        static string GetProject(string location)
        {
            var files = Directory.GetFiles(location, "*.csproj");
            return files.Length == 1 ? files[0] : null;
        }

        static string GetReferenceAssmblie(XmlNode node, XmlNamespaceManager ns)
        {
            var dllName = node.InnerText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0] + ".dll";
            var fullPathWithFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);
            if (File.Exists(fullPathWithFileName))
            {
                return fullPathWithFileName;
            }
            else
            {
                return Path.Combine(_runtimeDirectory, dllName);
            }
        }

        static string GetFileCulture(string fileNameWithoutExtension)
        {
            string ext = Path.GetExtension(fileNameWithoutExtension);
            if (!string.IsNullOrWhiteSpace(ext))
            {
                try
                {
                    ext = ext.Substring(1);
                    ext = CultureInfo.GetCultureInfo(ext).Name;
                }
                catch (CultureNotFoundException)
                {
                    ext = string.Empty;
                }
            }
            return ext;
        }

        static Resource GetResourceLocation(string projectsLocation, string include, string rootNamespace)
        {
            string resourceName = string.Empty; var compile = false;

            var fileName = Path.GetFileName(include);
            var ext = Path.GetExtension(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var culture = GetFileCulture(fileNameWithoutExtension);

            fileName = Path.Combine(projectsLocation, include);
            if (".resx" == ext.ToLower())
            {
                compile = true;
                resourceName = rootNamespace + "." + include.Replace("\\", ".").Replace("/", ".").Replace(ext, ".resources");
            }
            else
            {
                include = rootNamespace + "." + include.Replace("\\", ".").Replace("/", ".");
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    resourceName = include.Replace("." + culture, "", StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    resourceName = include;
                }
            }
            return new Resource { Culture = culture, FileName = fileName, ResourceName = resourceName, Compile = compile };
        }

        public static Assembly Compile(CommandLine cmdLine, Action<string, object[]> errorHandler)
        {
            var projectsLocation = Config.ProjectsLocation;
            var assemblyName = Path.GetFileName(cmdLine.Assembly).Replace(".dll", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var project = assemblyName;

            List<string> files = null;
            List<string> referenceAssmblies = null;
            List<Resource> resources = null;

            projectsLocation = Path.Combine(projectsLocation, project);

            project = GetProject(projectsLocation);
            if (null == project)
            {
                errorHandler.ShowError("Can't find project file for {0}.", project);
                return null;
            }

            try
            {
                var xmlProject = new XmlDocument();
                var ns = new XmlNamespaceManager(xmlProject.NameTable);

                xmlProject.Load(project);
                ns.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

                var outputType = xmlProject.SelectSingleNode("//msbuild:OutputType", ns).GetNodeValue();
                if (string.IsNullOrWhiteSpace(outputType) || outputType != "Library")
                    throw new ApplicationException("Unsupported project type.");

                var appDesignerFolder = xmlProject.SelectSingleNode("//msbuild:AppDesignerFolder", ns).GetNodeValue();
                if (string.IsNullOrWhiteSpace(appDesignerFolder))
                    throw new ApplicationException("Can't be determined AppDesignerFolder.");

                var rootNamespace = xmlProject.SelectSingleNode("//msbuild:RootNamespace", ns).GetNodeValue();
                if (string.IsNullOrWhiteSpace(rootNamespace))
                    throw new ApplicationException("Can't be determined RootNamespace.");

                files = xmlProject.SelectNodes("//msbuild:Compile/@Include", ns).OfType<XmlNode>()
                    .Select(n => Path.Combine(projectsLocation, n.Value)).ToList();

                referenceAssmblies = new List<string>();
                referenceAssmblies.AddRange(xmlProject.SelectNodes("//msbuild:Reference/@Include", ns)
                    .OfType<XmlNode>()
                    .Select(n => GetReferenceAssmblie(n, ns)));

                referenceAssmblies.AddRange(xmlProject.SelectNodes("//msbuild:ProjectReference/msbuild:Name", ns)
                    .OfType<XmlNode>()
                    .Select(n => GetReferenceAssmblie(n, ns)));

                resources = xmlProject.SelectNodes("//msbuild:EmbeddedResource/@Include", ns).OfType<XmlNode>()
                    .Select(n => GetResourceLocation(projectsLocation, n.Value, rootNamespace))
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);

                errorHandler.ShowError("Invalid project file {0}. {1}", project, ex.Message);
                return null;
            }

            if (!referenceAssmblies.Where(f => f.EndsWith("mscorlib.dll", StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                referenceAssmblies.Insert(0, Path.Combine(_runtimeDirectory, "mscorlib.dll"));
            }

            if (null == files || 0 == files.Count)
            {
                errorHandler.ShowError("No source file is specified into task descriptor.");
                return null;
            }

            var compilator = new CSharpCompiler
            {
                ErrorHandler = errorHandler,
                Debug = cmdLine.Debug,
                AssemblyName = assemblyName,
                Files = files,
                Resources = resources,
                ReferenceAssemblies = referenceAssmblies
            };

            return compilator.Compile();
        }

        bool BuildSatelliteAssembly(string culture, string version, List<Resource> resources)
        {
            var path = Path.Combine(Config.TasksLocation, culture);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            var code = "using System.Reflection;";
            code += System.Environment.NewLine;
            code += string.Format("[assembly: AssemblyCulture(\"{0}\")]", culture);
            code += System.Environment.NewLine;
            code += string.Format("[assembly: AssemblyVersion(\"{0}\")]", version);

            var manifestResources = resources.Select(r => new ResourceDescription(
                    r.ResourceName,
                    () => File.OpenRead(r.CompiledResource ?? r.FileName),
                    true));

            var assemblyFileName = AssemblyName + ".resources";
            var compilation = CSharpCompilation.Create(assemblyFileName)
                .WithOptions(CreateCompilationOptions())
                .AddSyntaxTrees(ParseCode(code))
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(_runtimeDirectory, "mscorlib.dll")));

            var assemblyPath = Path.Combine(path, assemblyFileName + ".dll");
            var emitResult = compilation.Emit(assemblyPath, manifestResources: manifestResources);
            if (!emitResult.Success)
            {
                ShowErrors(emitResult);
                return false;
            }
            return true;
        }

        bool BuildSatelliteAssemblies(Dictionary<string, Resource[]> resourcesByLanguage, string version)
        {
            foreach (var i in resourcesByLanguage)
            {
                if (string.IsNullOrWhiteSpace(i.Key))
                {
                    continue;
                }
                if (!BuildSatelliteAssembly(i.Key, version, i.Value.ToList()))
                {
                    ErrorHandler.ShowError(string.Format("Can't compile resource in language '{0}'.", i.Key));
                    return false;
                }
            }
            return true;
        }

        bool CompileResources(out Dictionary<string, Resource[]> resourcesByLanguage)
        {
            resourcesByLanguage = Resources.GroupBy(e => e.Culture).ToDictionary(e => e.Key, e => e.ToArray(), StringComparer.InvariantCultureIgnoreCase);
            foreach (var r in Resources.Where(r => r.Compile))
            {
                try
                {
                    string compiledResource = null;
                    if (!ResXCompiler.Compile(r.FileName, ref compiledResource))
                    {
                        ErrorHandler.ShowError(string.Format("Can't compile resource '{0}'.", r.FileName));
                        return false;
                    }
                    r.CompiledResource = compiledResource;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    ErrorHandler.ShowError(string.Format("Compiling resources: {0}.", r.FileName) + ex.Message);
                    return false;
                }
            }
            return true;
        }

        static void Clear(string location, string pattern)
        {
            foreach (var dir in Directory.EnumerateDirectories(location))
                Clear(dir, pattern);

            foreach (var file in Directory.EnumerateFiles(location, pattern))
                File.Delete(file);
        }

        void Clear()
        {
            if (!Directory.Exists(Config.TasksLocation))
            {
                Directory.CreateDirectory(Config.TasksLocation);
                return;
            }
            Clear(Config.TasksLocation, Path.GetFileNameWithoutExtension(AssemblyName) + "*.*");
        }

        static SyntaxTree Parse(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var stringText = SourceText.From(stream, Encoding.UTF8);
                return SyntaxFactory.ParseSyntaxTree(stringText, _parserOptions, filename);
            }
        }

        static SyntaxTree ParseCode(string text)
        {
            var stringText = SourceText.From(text);
            return SyntaxFactory.ParseSyntaxTree(stringText, _parserOptions);
        }

        CSharpCompilationOptions CreateCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(Debug ? OptimizationLevel.Debug : OptimizationLevel.Release)
                .WithWarningLevel(0)
                .WithPlatform(Platform.AnyCpu)
                .WithGeneralDiagnosticOption(ReportDiagnostic.Error);
        }

        void WriteTo(MemoryStream stream, string fileName)
        {
            stream.Position = 0;
            using (var fileStream = File.OpenWrite(fileName))
            {
                var buffer = new byte[32 * 1024];
                var read = 0;
                while (0 != (read = stream.Read(buffer, 0, buffer.Length)))
                {
                    fileStream.Write(buffer, 0, read);
                }
            }
        }

        IEnumerable<ResourceDescription> CreateAssemblyResources(Dictionary<string, Resource[]> resourcesByLanguage)
        {
            try
            {
                return resourcesByLanguage[""].Select(r => new ResourceDescription(
                    r.ResourceName,
                    ()=> File.OpenRead(r.CompiledResource??r.FileName),
                    true));
            }
            catch (KeyNotFoundException)
            {
                return Enumerable.Empty<ResourceDescription>();
            }
        }

        void ShowErrors(EmitResult emitResult)
        {
            foreach (Diagnostic d in emitResult.Diagnostics)
            {
                ErrorHandler.ShowError(d.GetMessage());
            }
        }

        Assembly Compile()
        {
            Dictionary<string, Resource[]> resourcesByLanguage;
            Assembly assembly = null;
            try
            {
                Clear();

                if (!CompileResources(out resourcesByLanguage))
                {
                    return null;
                }

                var manifestResources = CreateAssemblyResources(resourcesByLanguage);

                var compilation = CSharpCompilation.Create(AssemblyName)
                    .WithOptions(CreateCompilationOptions())
                    .AddSyntaxTrees(Files.Select(f => Parse(f)))
                    .AddReferences(ReferenceAssemblies.Select(f => MetadataReference.CreateFromFile(f)));

                var emitResult = (EmitResult)null;
                var assemblyFileName = AssemblyName + ".dll";
                var assemblyPath = Path.Combine(Config.TasksLocation, assemblyFileName);
                var assemblyPdbFileName = Path.Combine(Config.TasksLocation, AssemblyName + ".pdb");
                if (Debug)
                {
                    emitResult = compilation.Emit(assemblyPath, assemblyPdbFileName, manifestResources: manifestResources);
                }
                else
                {
                    emitResult = compilation.Emit(assemblyPath, manifestResources: manifestResources);
                }

                if (emitResult.Success)
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                    if (!BuildSatelliteAssemblies(resourcesByLanguage, assembly.GetName().Version.ToString()))
                    {
                        assembly = null;
                    }
                }
                else
                {
                    ShowErrors(emitResult);
                }
            }
            catch (Exception ex)
            {
                assembly = null;
                Log.Exception(ex);
            }
            finally
            {
                if (assembly == null)
                {
                    Clear();
                }
                foreach (var res in Resources.Where(res => (res.Compile && File.Exists(res.CompiledResource))))
                    File.Delete(res.CompiledResource);
            }
            return assembly;
        }
    }
}