using CSRunner.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CSRunner
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        IServiceInstallers serviceInstallers;

        public ServiceInstaller()
        {
            InitializeComponent();

            serviceInstallers = Program.CreateServiceInstallers();

            Installers.AddRange(serviceInstallers.Get() as Installer[]);

            this.BeforeInstall += ServiceInstaller_BeforeInstall;
        }

        void ServiceInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            var commandLine = Context.Parameters["commandline"] ?? "";
            var assemblyPath = Context.Parameters["assemblypath"];

            serviceInstallers.Initialize(Context.Parameters);

            Context.Parameters["assemblypath"] = string.Format("\"{0}\" {1}",
                assemblyPath, commandLine).TrimEnd();
        }
    }
}
