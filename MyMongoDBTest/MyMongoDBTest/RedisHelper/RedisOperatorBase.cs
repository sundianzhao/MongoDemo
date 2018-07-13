using System;
using System.Collections.Generic;
using ServiceStack.Redis; 

namespace MyMongoDBTest.RedisHelper
{
    public abstract class RedisOperatorBase : IDisposable
    {
        protected IRedisClient Redis { get; private set; }
        private bool _disposed = false;
        protected RedisOperatorBase()
        {
            Redis = RedisManager.GetClient();
        }
        protected RedisOperatorBase(long _redisdb)
        {
            Redis = RedisManager.GetClient(_redisdb);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (Redis != null)
                    {
                        Redis.Dispose();
                        Redis = null;
                    }
                    this._disposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        public void Save()
        {
            Redis.Save();
        }
        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            Redis.SaveAsync();
        }

        public bool Ping()
        {
            var client = Redis as RedisClient;
            return client.Ping();
        }

        public HashSet<string> GetUnionFromSets(params string[] setIds)
        {
            return Redis.GetUnionFromSets(setIds);
        }
    }
}