using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    public class CommandLine
    {
        public static CommandLine CreateDefault()
        {
            var cmdLine = new CommandLine
            {
                TaskUserID = Config.TaskUserID,
                Standalone = Config.Standalone,
                Server = Config.Server,
                Threads = Config.Threads,
                PrintFolder = Config.PrintFolder,
                LogFolder = Config.LogFolder,
                SleepInterval = Config.SleepInterval,
                EndOfDay = Config.EndOfDay,
                AccountingAuth = true,
                AccountingAsync = true,
                Parameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase),
                //CultureInfo = PRCB.Globalization.BGCultureInfo.CreateCulture(Config.Culture, false),
                Smtp = Config.Smtp,
                EmailFrom = Config.EmailFrom
            };
            return cmdLine;
        }

        internal bool Checked
        {
            get;
            set;
        }

        public bool Compile
        {
            get;
            internal set;
        }

        public CultureInfo CultureInfo
        {
            get;
            internal set;
        }

        public bool Debug
        {
            get;
            internal set;
        }

        public bool Standalone
        {
            get;
            internal set;
        }
        
        public bool Service
        {
            get;
            internal set;
        }

        public long TaskUserID
        {
            get;
            internal set;
        }

        public string Assembly
        {
            get;
            internal set;
        }

        public string Task
        {
            get;
            internal set;
        }

        public int StID
        {
            get;
            internal set;
        }

        public string Server
        {
            get;
            internal set;
        }

        public string User
        {
            get;
            internal set;
        }

        public bool UseDB
        {
            get;
            internal set;
        }

        public string PrintFolder
        {
            get;
            internal set;
        }

        public string LogFolder
        {
            get;
            internal set;
        }

        public bool AccountingAsync
        {
            get;
            internal set;
        }

        public bool AccountingAuth
        {
            get;
            internal set;
        }

        public byte Threads
        {
            get;
            internal set;
        }

        public TimeSpan SleepInterval
        {
            get;
            internal set;
        }

        public bool EndOfDay
        {
            get;
            internal set;
        }

        public Config.SmtpConfig Smtp
        {
            get;
            internal set;
        }

        public MailAddress EmailFrom
        {
            get;
            internal set;
        }        

        public Dictionary<string, object> Parameters
        {
            get;
            internal set;
        }
    }
}