using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    public class Resource
    {
        public string Culture
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public string ResourceName
        {
            get;
            set;
        }

        public string CompiledResource
        {
            get;
            set;
        }

        public bool Compile
        {
            get;
            set;
        }
    }
}
