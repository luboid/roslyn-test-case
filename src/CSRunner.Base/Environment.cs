
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CSRunner.Base
{
    public class Environment : ICloneable, IDisposable
    {
        internal bool signaled = false;
        internal Action<Environment> nextIteration = null;

        public int Index
        {
            get;
            internal set;
        }

        public bool Master
        {
            get
            {
                return 0 == Index;
            }
        }

        //public Database Database
        //{
        //    get;
        //    internal set;
        //}

        public CommandLine CommandLine
        {
            get;
            internal set;
        }

        public Events Events
        {
            get;
            internal set;
        }

        //public IRecordReader RecordReader
        //{
        //    get;
        //    set;
        //}

        //public IRecordCounter RecordCounter
        //{
        //    get;
        //    set;
        //}

        public bool Running
        {
            get
            {
                if (null != nextIteration)
                    nextIteration(this);

                return !CancellationTokenSource.IsCancellationRequested;
            }
        }

        public CancellationToken Token
        {
            get
            {
                return CancellationTokenSource.Token;
            }
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
        }

        //public int[] ExitCodes()
        //{
        //    return ThreadsContext.ExitCodes();
        //}

        //public bool Wait()
        //{
        //    ThreadsContext.Wait(this);
        //    return Master;
        //}

        public object Clone()
        {
            var env = new CSRunner.Base.Environment();
            env.nextIteration = nextIteration;
            env.CancellationTokenSource = CancellationTokenSource;
            //env.Database = Database.Clone() as Database;
            env.CommandLine = CommandLine;
            env.Events = Events;
            //env.RecordReader = RecordReader;
            //env.RecordCounter = RecordCounter;
            //env.ThreadsContext = ThreadsContext;
            return env;
        }

        internal CancellationTokenSource CancellationTokenSource
        {
            get;
            set;
        }

        //internal ThreadsContext ThreadsContext
        //{
        //    get;
        //    set;
        //}

        public void Dispose()
        {
            if (Master)
            {
                //if (null != RecordReader)
                //    RecordReader.Dispose();

                //if (null != ThreadsContext.CountdownEvent)
                //    ThreadsContext.CountdownEvent.Dispose();

                if (null != CancellationTokenSource)
                    CancellationTokenSource.Dispose();

                if (null != Events)
                    Events.Dispose();
            }

            //if (null != Database)
            //    Database.Dispose();
        }
    }
}
