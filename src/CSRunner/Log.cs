using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner
{
    class Log
    {
        //public static NLog.Logger Logger = NLog.LogManager.GetLogger("CSRunner.Program");

        public static void Exception(Exception ex)
        {
            System.Console.WriteLine(ex.ToString());
            //Logger.ErrorException("Unexpected exception", ex);
        }
    }
}