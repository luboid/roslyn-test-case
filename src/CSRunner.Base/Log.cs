using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    class Log
    {
        //public static NLog.Logger Logger = NLog.LogManager.GetLogger("CSRunner.Base");

        public static void Exception(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            //Logger.ErrorException("Unexpected exception", ex);
        }
    }
}