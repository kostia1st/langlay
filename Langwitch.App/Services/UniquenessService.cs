using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Langwitch
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
