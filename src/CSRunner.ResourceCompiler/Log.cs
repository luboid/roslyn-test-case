using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.ResourceCompiler
{
    class Log
    {
        //public static NLog.Logger Logger = NLog.LogManager.GetLogger("CSRunner.ResourceCompiler");

        public static void Exception(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            //Logger.ErrorException("Unexpected exception", ex);
        }
    }
}