using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRunner.Base
{
    public class Runner : IDisposable
    {
        protected bool disposed;
        protected int exitCode;
        protected Environment environment;

        public Runner(Environment environment)
        {
            this.environment = environment;
        }

        public Environment Environment
        {
            get
            {
                return this.environment;
            }
        }

        protected bool Running
        {
            get
            {
                return this.environment.Running;
            }
        }

        public virtual void Info(string message, params object[] parameters)
        {
            this.environment.Events.Info(message, parameters);
        }

        public virtual void Error(string message, params object[] parameters)
        {
            this.environment.Events.Error(message, parameters);
        }

        public virtual int ExitCode
        {
            get
            {
                return exitCode;
            }
            set
            {
                exitCode = value;
            }
        }

        public virtual void Run()
        { }

        public virtual bool CheckParameters()
        {
            return true;
        }

        public virtual bool BeforeRun()
        {
            return true;
        }

        public virtual void AfterRun()
        { }

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

        ~Runner()
        {
            Dispose(false);
        }
    }
}