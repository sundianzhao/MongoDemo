using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace MyMongoDBTest.RedisHelper
{
    public class ListOperator : RedisOperatorBase
    {
        public ListOperator()
            : base()
        { }
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool Exist<T>(string setId, string item)
        {
            return Redis.SetContainsItem(setId, item);
        }

        /// <summary>
        /// 将实体推入队列
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="listId">队列id</param>
        /// <param name="item">实体实例</param>
        public void EnqueueItemOnList<T>(string listId, T item)
        {
            var value = JsonSerializer.SerializeToString<T>(item);
            Redis.EnqueueItemOnList(listId,value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="values"></param>
        public void AddRangeToSet(string setId, List<string> values)
        {
            Redis.AddRangeToSet(setId,values);
        }

        /// <summary>
        /// 存储数据到Set集合
        /// </summary>
        public void Add(string setId, string value)
        {
            Redis.AddItemToSet(setId, value);
        }
        /// <summary>
        /// 移除Set中的某值
        /// </summary>
        public void Remove(string setId, string value)
        {
             Redis.RemoveItemFromSet(setId, value);
        }
        /// <summary>
        /// 移除整个Set
        /// </summary>
        public bool Remove(string key)
        {
            return Redis.Remove(key);
        }
        /// <summary>
        /// 从Set中获取数据获取id
        /// </summary>
        public  IEnumerable<string> Get(string setId)
        {
            return Redis.Sets[setId];
        }
        ///// <summary>
        ///// 获取整个hash的数据
        ///// </summary>
        //public List<T> GetAll<T>(string hashId)
        //{
        //    var result = new List<T>();
        //    var list = Redis.GetHashValues(hashId);
        //    if (list != null && list.Count > 0)
        //    {
        //        list.ForEach(x =>
        //        {
        //            var value = JsonSerializer.DeserializeFromString<T>(x);
        //            result.Add(value);
        //        });
        //    }
        //    return result;
        //}
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public void SetExpire(string key, DateTime datetime)
        {
            Redis.ExpireEntryAt(key, datetime);
        }


    }
}