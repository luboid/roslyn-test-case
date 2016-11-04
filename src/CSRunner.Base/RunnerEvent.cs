using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    internal delegate void RunnerEvent(RunnerEventType type, string message, object[] parameters);
}
