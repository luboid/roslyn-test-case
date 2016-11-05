using CSRunner.Base;
using CSRunner.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner
{
    public class Program
    {
        internal static IServiceInstallers CreateServiceInstallers()
        {
            //var t = Type.GetType("CSRunner.Service.Installers, CSRunner.Service", true);
            //var s = Activator.CreateInstance(t) as IServiceInstallers;
            return null;
        }

        static void OkScenario(CSRunner.Base.Environment env)
        {
            var assembly = CSharpCompiler.Compile(env.CommandLine, (string errors, object[] parameters) =>
            {
                System.Console.WriteLine(string.Format(errors, parameters));
            });

            var runner = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(Runner))).SingleOrDefault();
            var runnerInstance = Activator.CreateInstance(runner, new[] { env }) as Runner;

            runnerInstance.Run();
        }

        static void FailScenario(CSRunner.Base.Environment env)
        {
            var assembly = CSharpCompilerInMemory.Compile(env.CommandLine, (string errors, object[] parameters) =>
            {
                System.Console.WriteLine(string.Format(errors, parameters));
            });

            var runner = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(Runner))).SingleOrDefault();
            var runnerInstance = Activator.CreateInstance(runner, new[] { env }) as Runner;

            runnerInstance.Run();
        }

        public static int Main(string[] args)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //setup test environment
            var thread = System.Threading.Thread.CurrentThread;
            thread.CurrentUICulture = thread.CurrentCulture = new CultureInfo("en");

            var cmdLine = new CommandLine();
            var env = new CSRunner.Base.Environment();
            env.CommandLine = cmdLine;
            env.Events = new Events();
            env.Events.RunnerEvent += (RunnerEventType type, string message, object[] parameters) =>
            {
                System.Console.WriteLine(type + ":" + string.Format(message, parameters));
            };

            cmdLine.Assembly = "Documents.Queue2";// project to compile
            cmdLine.Debug = true;

            // test scenarios
            // Here satellite assembly is loaded because all written to then files and assembly
            // receives its Location, and message is showing in English
            // OkScenario(env);

            // Case with problems, assemblies are only in memory and can't paired
            // Here message is not shown in English
            FailScenario(env);//Working with AppDomain.AssemblyResolve 

            return 0;
        }

        //private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    //var assmebly = new AssemblyName(args.Name);
        //    //if (assmebly.Name.Contains(".resources"))
        //    //{
        //    //    var path = Path.Combine(Config.TasksLocation, "en\\" + assmebly.Name);
        //    //    return Assembly.LoadFrom(path);
        //    //}
        //    System.Console.WriteLine(args.Name);
        //    return null;
        //}
    }
}
