using System.Linq;
using ServiceStack.Redis; 

namespace MyMongoDBTest.RedisHelper
{
    public class RedisManager
    {
        /// <summary>  
        /// redis配置文件信息  
        /// </summary>  
        private static RedisConfigInfo redisConfigInfo = RedisConfigInfo.GetConfig();

        private static PooledRedisClientManager prcm;
        private static int _db;
        /// <summary>  
        /// 静态构造方法，初始化链接池管理对象  
        /// </summary>  
        static RedisManager()
        {
            CreateManager();
        }


        /// <summary>  
        /// 创建链接池管理对象  
        /// </summary>  
        private static void CreateManager()
        {

            //var host = HttpContext.Current.Request.Url.Host;
            //var configEntity = db.w_webconfig.FirstOrDefault(w=>host.EndsWith(w.dmain));
            //if (configEntity == null)
            //{
            //    LogHelper.WriteErrorLog("获取Redis-webconfig失败");
            //    throw new NullReferenceException("获取webconfig失败!");
            //}
            //var configStr = configEntity.connection_string;
            var config = redisConfigInfo;
            _db = config.Db;
            string[] writeServerList = SplitString(config.WriteServerList, ",");
            string[] readServerList = SplitString(config.ReadServerList, ",");
            prcm = new PooledRedisClientManager(readServerList, writeServerList,
                             new RedisClientManagerConfig
                             {
                                 MaxWritePoolSize = config.MaxWritePoolSize,
                                 MaxReadPoolSize = config.MaxReadPoolSize,
                                 AutoStart = config.AutoStart,
                             });
        }

        private static string[] SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }


        /// <summary>  
        /// 客户端缓存操作对象  
        /// </summary>  
        public static IRedisClient GetClient()
        {
            if (prcm == null)
                CreateManager();
            var client = prcm.GetClient();
            client.Db = _db;
            return client;
        }

        /// <summary>  
        /// 客户端缓存操作对象  
        /// </summary>  
        public static IRedisClient GetClient(long _redisdb)
        {
            if (prcm == null)
                CreateManager();
            var client = prcm.GetClient();
            client.Db = _redisdb;
            return client;
        }
        private class JsonConfig
        {
            public string WriteServerList { get; set; }
            public string ReadServerList { get; set; }
            public int MaxWritePoolSize { get; set; }
            public int MaxReadPoolSize { get; set; }
            public bool AutoStart { get; set; }
            public int LocalCacheTime { get; set; }
            public bool RecordeLog { get; set; }
        }
    }
}