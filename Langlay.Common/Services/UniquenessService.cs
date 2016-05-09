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
            bool isTaken;
            using (var mutex = new Mutex(true, UniqueId, out isTaken))
            {
                if (!isTaken && DoForceThisInstance)
                {
                    CloseConcurrentMutexHandler?.Invoke();
                    mutex.WaitOne();
                    isTaken = true;
                }
                if (isTaken)
                    action();
            }
            return isTaken;
        }
    }
}
