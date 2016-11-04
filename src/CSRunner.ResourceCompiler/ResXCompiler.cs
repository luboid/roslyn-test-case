using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CSRunner.Base;

namespace CSRunner.ResourceCompiler
{
    public class ResXCompiler : IResXCompiler
    {
        public bool Compile(string resx, ref string resources)
        {
            DictionaryEntry item;
            if (string.IsNullOrWhiteSpace(resources))
            {
                resources = Path.GetTempFileName();
            }

            try
            {
                using (var w = new ResourceWriter(resources))
                {
                    using (var r = new ResXResourceReader(resx))
                    {
                        r.BasePath = Path.GetDirectoryName(resx);
                        var e = r.GetEnumerator();
                        while (e.MoveNext())
                        {
                            item = (DictionaryEntry)e.Current;
                            w.AddResource(item.Key as string, item.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
            return true;
        }

        //public bool BuildAssembly(string location, string culture, IList<Resource> resources, bool debug = false)
        //{
        //    //http://stackoverflow.com/questions/8634764/build-resource-assemblies-with-assemblybuilder

        //    string path = Path.GetDirectoryName(location);
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    string assemblyName = Path.GetFileName(location);

        //    DictionaryEntry item;

        //    AppDomain appDomain = Thread.GetDomain();// AppDomain.CreateDomain("DefineDynamicAssembly ...");
        //    try
        //    {
        //        AssemblyName asmName = new AssemblyName();
        //        asmName.Name = assemblyName;
        //        asmName.CodeBase = path;
        //        asmName.CultureInfo = new CultureInfo(culture);

        //        AssemblyBuilder myAsmBuilder = appDomain.DefineDynamicAssembly(
        //            asmName,
        //            AssemblyBuilderAccess.RunAndSave, path);

        //        /*var a = myAsmBuilder.GetCustomAttributes(typeof(AssemblyVersionAttribute)).FirstOrDefault() as AssemblyVersionAttribute;

        //        ConstructorInfo infoAttrCtor =
        //             typeof(AssemblyVersionAttribute).GetConstructor(new Type[] { typeof(string) });
        //        CustomAttributeBuilder infoAttr =
        //           new CustomAttributeBuilder(infoAttrCtor, new object[] { "1.0.0.0" });
        //        myAsmBuilder.SetCustomAttribute(infoAttr);*/

        //        ModuleBuilder myModuleBuilder = myAsmBuilder.DefineDynamicModule("Module", assemblyName);

        //        foreach (var r in resources)
        //        {
        //            if (r.Compile)
        //            {
        //                var w = myModuleBuilder.DefineResource(r.ResourceName, string.Empty, ResourceAttributes.Public);
        //                using (var reader = new ResXResourceReader(r.FileName))
        //                {
        //                    reader.BasePath = Path.GetDirectoryName(r.FileName);
        //                    var e = reader.GetEnumerator();
        //                    while (e.MoveNext())
        //                    {
        //                        item = (DictionaryEntry)e.Current;
        //                        w.AddResource(item.Key as string, item.Value);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                myModuleBuilder.DefineManifestResource(r.ResourceName, File.Open(r.FileName, FileMode.Open), ResourceAttributes.Public);
        //            }
        //        }

        //        myAsmBuilder.Save(assemblyName);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        return false;
        //    }
        //    finally
        //    {
        //        //AppDomain.Unload(appDomain);
        //    }
        //    return true;
        //}
    }
}