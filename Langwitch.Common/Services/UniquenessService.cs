using System;
using System.Threading;

namespace Product.Common
{
    public class UniquenessService
    {
        private string UniqueId { get; set; }
        public UniquenessService(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        public bool RunOrIgnore(Action action)
        {
            bool isTaken;
            using (var mutex = new Mutex(true, UniqueId, out isTaken))
            {
                if (isTaken)
                    action();
            }
            return isTaken;
        }
    }
}
