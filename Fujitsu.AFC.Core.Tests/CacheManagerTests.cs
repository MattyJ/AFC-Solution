using System.Linq;
using System.Runtime.Caching;
using Fujitsu.AFC.Core.Caching;
using Fujitsu.AFC.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Core.Tests
{
    [TestClass]
    public class CacheManagerTests
    {
        private ICacheManager _target;
        private MemoryCache _cache;
        private const string CacheKey = "XXX";

        [TestInitialize]
        public void Initialize()
        {
            _cache = MemoryCache.Default;
            var cacheKeys = _cache.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
            _target = new CacheManager();
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ReferenceItemNotInCache_ReturnsResults()
        {
            var result = _target.ExecuteAndCache(CacheKey, GetClass);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TestCacheClass));
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ReferenceItemNotInCache_ItemAddedToCache()
        {
            _target.ExecuteAndCache(CacheKey, GetClass);
            var cacheItem = _cache.Get(CacheKey);
            Assert.IsNotNull(cacheItem);
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ReferenceItemInCache_ItemNotAddedAgain()
        {
            _target.ExecuteAndCache(CacheKey, GetClass);
            var result = _target.ExecuteAndCache(CacheKey, GetClassDifferent);
            Assert.IsNotNull(result);
            Assert.AreEqual("XXX", result.Name);
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ValueItemNotInCache_ReturnsResults()
        {
            var result = _target.ExecuteAndCache(CacheKey, GetInt);
            Assert.AreNotEqual(0, result);
            Assert.IsInstanceOfType(result, typeof(int));
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ValueItemNotInCache_ItemAddedToCache()
        {
            _target.ExecuteAndCache(CacheKey, GetInt);
            var cacheItem = _cache.Get(CacheKey);
            Assert.AreNotEqual(0, cacheItem);
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ValueItemInCache_ItemNotAddedAgain()
        {
            _target.ExecuteAndCache(CacheKey, GetInt);
            var result = _target.ExecuteAndCache(CacheKey, GetIntDifferent);
            Assert.AreNotEqual(0, result);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void CacheManager_Remove_ValueItemInCache_ItewmIsRemoved()
        {
            _target.ExecuteAndCache(CacheKey, GetInt);
            _target.Remove(CacheKey);
            Assert.IsFalse(_cache.Contains(CacheKey));
        }

        private static TestCacheClass GetClass()
        {
            return new TestCacheClass
            {
                Name = "XXX"
            };
        }

        private static TestCacheClass GetClassDifferent()
        {
            return new TestCacheClass
            {
                Name = "YYY"
            };
        }

        private static int GetInt()
        {
            return 10;
        }

        private static int GetIntDifferent()
        {
            return 100;
        }

        private class TestCacheClass
        {
            public string Name { get; set; }
        }
    }
}
