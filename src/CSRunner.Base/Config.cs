using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;

namespace CSRunner.Base
{
    public static class Config
    {
        public class SmtpConfig
        {
            public int Port
            {
                get;
                internal set;
            }

            public string Host
            {
                get;
                internal set;
            }

            internal static bool TryParse(string smtp, out SmtpConfig config)
            {
                config = null;
                if (string.IsNullOrWhiteSpace(smtp))
                {
                    return false;
                }
                else
                {
                    var parts = (smtp ?? string.Empty).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (1 == parts.Length)
                    {
                        config = new SmtpConfig { Host = parts[0], Port = 25 };
                    }
                    else
                    {
                        int port;
                        if (!int.TryParse(parts[1], out port))
                        {
                            return false;
                        }

                        config = new SmtpConfig { Host = parts[0], Port = port };
                    }
                }
                return true;
            }
        }

        static Type _resXCompiler;

        static Config()
        {
            SmtpConfig cfg; long l; bool b; TimeSpan t; string s;
            ProjectsLocation = ConfigurationManager.AppSettings["packagesLocation"];
            TasksLocation = ConfigurationManager.AppSettings["tasksLocation"];
            Server = ConfigurationManager.AppSettings["server"];

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["console.unicode"]) && bool.TryParse(ConfigurationManager.AppSettings["console.unicode"], out b))
                Unicode = b;
            else
                Unicode = true;

            s = ConfigurationManager.AppSettings["emailFrom"];
            if (!string.IsNullOrWhiteSpace(s))
            {
                try
                {
                    EmailFrom = new MailAddress(s);
                }
                catch(Exception ex)
                {
                    Log.Exception(ex);
                }
            }

            s = ConfigurationManager.AppSettings["smtp"];
            if (!string.IsNullOrWhiteSpace(s))
            {
                if (SmtpConfig.TryParse(s, out cfg))
                {
                    Smtp = cfg;
                }
                else
                {
                    Console.Error.WriteLine(string.Format("Invalid smtp configuration {0}.", s));
                    //Log.Logger.Error<string>(string.Format("Invalid smtp configuration {0}.", s));
                }
            }

            if (string.IsNullOrWhiteSpace(ProjectsLocation))
                ProjectsLocation = DefaultProjectsLocation;
            if (string.IsNullOrWhiteSpace(TasksLocation))
                TasksLocation = DefaultTasksLocation;

            Culture = ConfigurationManager.AppSettings["culture"] ?? "bg";
            CompilerVersion = ConfigurationManager.AppSettings["compilerVersion"] ?? "v4.0";

            if (!long.TryParse(ConfigurationManager.AppSettings["taskUserID"] ?? "999999", out l))
                l = 0L;

            TaskUserID = l;

            if (!long.TryParse(ConfigurationManager.AppSettings["threads"] ?? "0", out l))
                l = 0L;

            Threads = (byte)l;

            if (!bool.TryParse(ConfigurationManager.AppSettings["standalone"] ?? "false", out b))
                b = false;

            Standalone = b;

            if (!bool.TryParse(ConfigurationManager.AppSettings["endofday"] ?? "false", out b))
                b = false;

            EndOfDay = b;

            if (TimeSpan.TryParse(ConfigurationManager.AppSettings["sleepinterval"], out t))
                SleepInterval = t;
            else
                SleepInterval = TimeSpan.FromSeconds(5);

            if (TimeSpan.TryParse(ConfigurationManager.AppSettings["databasesleepinterval"], out t))
                DatabaseSleepInterval = t;
            else
                DatabaseSleepInterval = TimeSpan.FromMinutes(3);

            if (TimeSpan.TryParse(ConfigurationManager.AppSettings["endofdaysleepinterval"], out t))
                EndOfDaySleepInterval = t;
            else
                EndOfDaySleepInterval = TimeSpan.FromMinutes(30);

            LogFolder = ConfigurationManager.AppSettings["log_folder"];
            PrintFolder = ConfigurationManager.AppSettings["print_folder"];
        }

        public static string DefaultProjectsLocation
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
            }
        }

        public static string DefaultTasksLocation
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks");
            }
        }


        public static string ProjectsLocation
        {
            get;
            private set;
        }

        public static string TasksLocation
        {
            get;
            private set;
        }

        public static long TaskUserID
        {
            get;
            private set;
        }

        public static bool Standalone
        {
            get;
            private set;
        }

        public static string CompilerVersion
        {
            get;
            private set;
        }

        public static bool EndOfDay
        {
            get;
            private set;
        }

        public static string Server
        {
            get;
            private set;
        }

        public static byte Threads
        {
            get;
            private set;
        }

        public static string LogFolder
        {
            get;
            private set;
        }

        public static string PrintFolder
        {
            get;
            private set;
        }

        public static string Culture
        {
            get;
            private set;
        }

        public static TimeSpan SleepInterval
        {
            get;
            private set;
        }

        public static TimeSpan DatabaseSleepInterval
        {
            get;
            private set;
        }

        public static TimeSpan EndOfDaySleepInterval
        {
            get;
            private set;
        }

        public static SmtpConfig Smtp
        {
            get;
            private set;
        }

        public static MailAddress EmailFrom
        {
            get;
            private set;
        }

        public static bool Unicode
        {
            get;
            private set;
        }

        public static Type ResXCompiler
        {
            get
            {
                if (null == _resXCompiler)
                {
                    var type = ConfigurationManager.AppSettings["resXCompiler"] ?? "CSRunner.ResourceCompiler.ResXCompiler, CSRunner.ResourceCompiler";
                    _resXCompiler = Type.GetType(type, true);
                    if (_resXCompiler.IsAssignableFrom(typeof(IResXCompiler)))
                    {
                        throw new ApplicationException("Type '{0}' don't support IResXCompiler interface.");
                    }
                }
                return _resXCompiler;
            }
        }
    }
}