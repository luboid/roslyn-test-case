using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    public interface IResXCompiler
    {
        bool Compile(string resx, ref string resources);
    }
}
