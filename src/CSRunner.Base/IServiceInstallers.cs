using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    public interface IServiceInstallers
    {
        object[] Get();
        void Initialize(StringDictionary parameters);
    }
}
