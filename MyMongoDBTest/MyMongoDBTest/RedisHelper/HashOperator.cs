using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text; 

namespace MyMongoDBTest.RedisHelper
{
    public class HashOperator : RedisOperatorBase
    {
        public HashOperator() : base() { }
        public HashOperator(long _redisdb) : base(_redisdb) { }
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool Exist<T>(string hashId, string key)
        {
            return Redis.HashContainsEntry(hashId, key);
        }
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public bool Set<T>(string hashId, string key, T t)
        {
            var value = JsonSerializer.SerializeToString<T>(t);
            return Redis.SetEntryInHash(hashId, key, value);
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public bool Remove(string hashId, string key)
        {
            return Redis.RemoveEntryFromHash(hashId, key);
        }

        public bool RemoveEntryFromHash(string hashId, params string[] keys)
        {
            var result = false;
            try
            {
                foreach (var key in keys)
                {
                    result = Redis.RemoveEntryFromHash(hashId, key);
                }
                return result;
            }
            catch (Exception ex)
            {
                //LogHelper.Error("删除某个redis中的某个键时出现异常", OpType.System, ex);
                return false;
            }
        }


        public Dictionary<string, T> GetAllEntriesFromHash<T>(string hashId)
        {
            var dic = Redis.GetAllEntriesFromHash(hashId);
            var result = new Dictionary<string, T>();
            foreach (var item in dic)
            {
                var value = JsonSerializer.DeserializeFromString<T>(item.Value);
                result.Add(item.Key, value);
            }
            return result;
        }


        /// <summary>
        /// 移除整个hash
        /// </summary>
        public bool Remove(string key)
        {
            return Redis.Remove(key);
        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        public T Get<T>(string hashId, string key)
        {
            //var time1=new Stopwatch();
            //var time2 =new Stopwatch();
            string value = Redis.GetValueFromHash(hashId, key);
            //time1.Start();
            var result = JsonSerializer.DeserializeFromString<T>(value);
            //time1.Stop();
            //time2.Start();
            ////var result = fastJSON.JSON.ToObject<T>(value); //
            //time2.Stop();
            //var s1 = time1.Elapsed;
            //var s2 = time2.Elapsed;
            return result;
        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        public IQueryable<T> GetAll<T>(string hashId)
        {
            var result = new List<T>();
            var list = Redis.GetHashValues(hashId);
            if (list != null && list.Count > 0)
            {
                list.ForEach(x =>
                {
                    var value = JsonSerializer.DeserializeFromString<T>(x);
                    result.Add(value);
                });
            }
            return result.AsQueryable();
        }
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public void SetExpire(string key, DateTime datetime)
        {
            Redis.ExpireEntryAt(key, datetime);
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="itemList"></param>
        public void AddRange(string hashId, IEnumerable<KeyValuePair<string, string>> itemList)
        {
            Redis.SetRangeInHash(hashId, itemList);
        }
        public IQueryable<T> GetEntitiesByIds<T>(string hashId, params string[] keys)
        {

            var list = Redis.GetValuesFromHash(hashId, keys);

            var result = list.Select(JsonSerializer.DeserializeFromString<T>).ToList();

            //var list = keys.Select(id => Redis.GetValueFromHash(hashId, id))

            //    .Select(JsonSerializer.DeserializeFromString<T>).ToList();

            return result.AsQueryable();

        }


        public IQueryable<T> Get<T>(string hashId, params string[] keys)
        {

            var list = Redis.GetValuesFromHash(hashId, keys);

            var result = list.Select(JsonSerializer.DeserializeFromString<T>).ToList();

            //var list = keys.Select(id => Redis.GetValueFromHash(hashId, id))

            //    .Select(JsonSerializer.DeserializeFromString<T>).ToList();

            return result.AsQueryable();

        }



    }
}