using CSRunner.Base;
using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Documents.Queue2.Properties;
using System.Threading;
using System.Globalization;

namespace Documents
{
    public class Queue : Runner
    {
        public Queue(CSRunner.Base.Environment environment) :
            base(environment)
        {

        }

        public override void Run()
        {
            Info(CultureInfo.CurrentCulture.Name);
            Info(Resources.Test);
        }
    }
}