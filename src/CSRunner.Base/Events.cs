using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    public class Events : IDisposable
    {
        protected bool disposed;

        internal Events()
        { }

        event RunnerEvent runnerEvent;

        internal event RunnerEvent RunnerEvent
        {
            add
            {
                this.runnerEvent += value;
            }
            remove
            {
                this.runnerEvent -= value;
            }
        }

        public void Info(string message, params object[] parameters)
        {
            if (null != runnerEvent)
                runnerEvent(RunnerEventType.Info, message, parameters);
        }

        public void Error(string message, params object[] parameters)
        {
            if (null != runnerEvent)
                runnerEvent(RunnerEventType.Error, message, parameters);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            /*if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    //this.environment.Dispose();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }*/
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        ~Events() 
        {
            Dispose(false);
        }
    }
}
