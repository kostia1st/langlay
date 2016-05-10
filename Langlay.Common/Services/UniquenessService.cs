using System;
using System.Threading;

namespace Product.Common
{
    public class UniquenessService
    {
        private string UniqueId { get; set; }
        private bool DoForceThisInstance { get; set; }
        private Action CloseConcurrentMutexHandler { get; set; }

        public UniquenessService(
            string uniqueId, bool forceThisInstance = false, 
            Action closeConcurrentMutexHandler = null)
        {
            UniqueId = uniqueId;
            DoForceThisInstance = forceThisInstance;
            CloseConcurrentMutexHandler = closeConcurrentMutexHandler;
        }

        public bool Run(Action action)
        {
            var isTaken = false;
            var mutex = new Mutex(false, UniqueId);
            try
            {
                isTaken = mutex.WaitOne(500);
                if (!isTaken && DoForceThisInstance)
                {
                    CloseConcurrentMutexHandler?.Invoke();
                    isTaken = mutex.WaitOne(500);
                }
                if (isTaken)
                    action();
            }
            finally
            {
                if (isTaken)
                    mutex.ReleaseMutex();
                mutex.Dispose();
            }
            return isTaken;
        }
    }
}
