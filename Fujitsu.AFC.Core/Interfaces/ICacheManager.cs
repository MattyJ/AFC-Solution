using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.AFC.Core.Interfaces
{
    public interface ICacheManager
    {
        TResult ExecuteAndCache<TResult>(string cacheItemKey, Func<TResult> underlyingGet);
        void Remove(string key);
    }
}
