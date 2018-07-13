using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;
using MyMongoDBTest.model;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System.Web.Script.Serialization;

namespace MyMongoDBTest
{
    class Program
    {
         static RedisClient client = new RedisClient("127.0.0.1", 6379);//redis服务IP和端口
        //static string host = "127.0.0.1";/*访问host地址*/
        //static string password = "2016@Msd.1127_kjy";/*实例id:密码*/
        //static  RedisClient client = new RedisClient("127.0.0.1", 6379);
        //static readonly RedisClient client = new RedisClient("xxxxx.m.cnsza.kvstore.aliyuncs.com", 6379, "dacb71347ad0409c:xxxx"); //49正式环境  
         static IRedisTypedClient<InStoreReceipt> redis = client.As<InStoreReceipt>();

        static void Main(string[] args)
        {
            try
            {
                RedisTestApp();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void RedisTestApp()
        {
            StringTest(); //字符串测试  

            HashTest(); //Hash测试  

            ObjectTest(); //实体对象测试  

            SingleObjEnqueueTest(); //单个对象队列测试  

            ListObjTest(); //对象列表测试  

            QueueTest(); //队列和栈测试  

            Console.ReadKey();
        }
        /// <summary>  
        /// 队列和栈测试  
        /// </summary>  
        private static void QueueTest()
        {
            Console.WriteLine("*******************队列 先进先出********************");

            client.EnqueueItemOnList("test", "饶成龙");//入队。  
            client.EnqueueItemOnList("test", "周文杰");
            long length = client.GetListCount("test");
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine(client.DequeueItemFromList("test"));//出队.  
            }
            Console.WriteLine("*********************栈 先进后出*****************");
            client.PushItemToList("name1", "邹琼俊");//入栈  
            client.PushItemToList("name1", "周文杰");
            long length1 = client.GetListCount("name1");
            for (int i = 0; i < length1; i++)
            {
                Console.WriteLine(client.PopItemFromList("name1"));//出栈.  
            }
            Console.ReadKey();
        }

        /// <summary>  
        /// 单个对象队列测试  
        /// </summary>  
        private static void SingleObjEnqueueTest()
        {
            Console.WriteLine("******************实体对象队列操作********************");
            Student _stu = new Student { Name = "张三", Age = 21 };
            JavaScriptSerializer json = new JavaScriptSerializer();
            client.EnqueueItemOnList("stu", json.Serialize(_stu));
            _stu = json.Deserialize<Student>(client.DequeueItemFromList("stu"));
            Console.WriteLine(string.Format("姓名：{0},年龄{1}", _stu.Name, _stu.Age));
            Console.ReadKey();
        }



        /// <summary>  
        /// List对象测试  
        /// </summary>  
        public static void ListObjTest()
        {
            List<InStoreReceipt> list = new List<InStoreReceipt>() { new InStoreReceipt() { IdentityID = 1, ReceiptStatus = 1, ReceiptTime = DateTime.Now, ReceiptMessage = "test1" },
            new InStoreReceipt() { IdentityID = 2, ReceiptStatus = 1, ReceiptTime = DateTime.Now, ReceiptMessage = "test2" },new InStoreReceipt() { IdentityID = 3, ReceiptStatus = 1, ReceiptTime = DateTime.Now, ReceiptMessage = "test3" }};
            AddInStoreInfo(list);
            var rList = redis.GetAllItemsFromList(redis.Lists["InStoreReceiptInfoList"]);
            rList.ForEach(v => Console.WriteLine(v.IdentityID + "," + v.ReceiptTime + "," + v.ReceiptMessage));
            redis.RemoveAllFromList(redis.Lists["InStoreReceiptInfoList"]);
            Console.ReadKey();
        }
        /// <summary>  
        /// 实体对象测试  
        /// </summary>  
        private static void ObjectTest()
        {
            Console.WriteLine("**************实体对象，单个，列表操作*****************");
            UserInfo userInfo = new UserInfo() { UserName = "zhangsan", UserPwd = "1111" };//</span>(底层使用json序列化 )    
            client.Set<UserInfo>("userInfo", userInfo);
            UserInfo user = client.Get<UserInfo>("userInfo");
            Console.WriteLine(user.UserName);

            //List<UserInfo> list = new List<UserInfo>() { new UserInfo() { UserName = "lisi", UserPwd = "222" }, new UserInfo() { UserName = "wangwu", UserPwd = "123" } };  
            //client.Set<List<UserInfo>>("list", list);  
            List<UserInfo> userInfoList = client.Get<List<UserInfo>>("list");
            userInfoList.ForEach(u => Console.WriteLine(u.UserName));
            client.Remove("list");

            Console.ReadKey();
        }

        /// <summary>  
        /// Hash测试  
        /// </summary>  
        private static void HashTest()
        {
            Console.WriteLine("********************Hash*********************");
            client.SetEntryInHash("userInfoId", "name", "zhangsan");
            var lstKeys = client.GetHashKeys("userInfoId");
            lstKeys.ForEach(k => Console.WriteLine(k));
            var lstValues = client.GetHashValues("userInfoId");
            lstValues.ForEach(v => Console.WriteLine(v));
            client.Remove("userInfoId");
            Console.ReadKey();
        }

        /// <summary>  
        /// 字符串测试  
        /// </summary>  
        private static void StringTest()
        {
            #region 字符串类型  
            Console.WriteLine("*******************字符串类型*********************");
            client.Set<string>("name", "zouqj");
            string userName = client.Get<string>("name");
            Console.WriteLine(userName);
            Console.ReadKey();
            #endregion
        }

        /// <summary>  
        /// 添加需要回执的进仓单信息到Redis  
        /// </summary>  
        /// <param name="lstRInStore">进仓单回执信息列表</param>  
        private static void AddInStoreInfo(List<InStoreReceipt> inStoreReceipt)
        {
            IRedisList<InStoreReceipt> rlstRInStore = redis.Lists["InStoreReceiptInfoList"];
            rlstRInStore.AddRange(inStoreReceipt);
        }

        /// <summary>  
        /// 进仓单回执信息（对应清关系统）  
        /// </summary>  
        public class InStoreReceipt
        {
            /// <summary>  
            /// 主键ID  
            /// </summary>  
            public int IdentityID { get; set; }
            /// <summary>  
            /// 回执状态  
            /// </summary>  
            public int ReceiptStatus { get; set; }
            /// <summary>  
            /// 回执时间  
            /// </summary>  
            public DateTime ReceiptTime { get; set; }
            /// <summary>  
            /// 回执信息  
            /// </summary>  
            public string ReceiptMessage { get; set; }
        }
        public class Student
        {
            /// <summary>  
            /// 姓名  
            /// </summary>  
            public string Name { get; set; }
            /// <summary>  
            /// 年龄  
            /// </summary>  
            public int Age { get; set; }
        }
        public class UserInfo
        {
            public string UserName { get; set; }
            public string UserPwd { get; set; }
        }
        static void RedisTest()
        {

            //将字符串列表添加到redis  
            //List<string> storeMembers = new List<string>() { "one", "two", "three" };
            //storeMembers.ForEach(x => redisClient.AddItemToList("additemtolist", x));

            //var list = redisClient.Lists["additemtolist"];
           // list.Clear();//清空  
           // list.Remove("two");//移除指定键值  


            ////得到指定的key所对应的value集合  

            //var members = redisClient.GetAllItemsFromList("additemtolist");
            //members.ForEach(s => Console.WriteLine("<br/>additemtolist :" + s));

            //// 获取指定索引位置数据   
            //var item = redisClient.GetItemFromList("addarrangetolist", 2);
            //Console.WriteLine(item);

            //存储对象（JSON序列化方法）它比object序列化方法效率高  
            //redisClient.Set<UserInfo>("userinfo", new UserInfo() { UserName = "李四", Age = 45 });
            //UserInfo userinfo = redisClient.Get<UserInfo>("userinfo");
            //Console.WriteLine("name=" + userinfo.UserName + "age=" + userinfo.Age);

            ////存储值类型数据  
            //redisClient.Set<int>("my_age", 12);//或Redis.Set("my_age", 12);  
            //int age = redisClient.Get<int>("my_age");
            //Console.WriteLine("age=" + age);

          

            ////也支持列表  
            //List<UserInfo> userinfoList = new List<UserInfo> {
            //new UserInfo{UserName="zzl",Age=1,Id=1},
            //new UserInfo{UserName="zhz",Age=3,Id=2},
            //};
            //redisClient.Set<List<UserInfo>>("userinfolist_serialize", userinfoList);
            //List<UserInfo> userList = redisClient.Get<List<UserInfo>>("userinfolist_serialize") ;
            //userList.ForEach(i =>
            //{
            //    Console.WriteLine("name=" + i.UserName + "age=" + i.Age);
            //});
        }
        static void MongoTest()
        {
            var helper = new MongoDbHelper<mongoCustomer>();
            //添加数据
            //var model = new mongoCustomer { Name = "名称", TaxCode = "adbc", AddressPhone = "17753633701", BankAccount = "12345" };
            //var m = helper.Insert(model);
            ////查询
            //var exp = ExtLinq.True<mongoCustomer>().And(p => p.Name == "名称");
            //var m1 = helper.QueryByFirst(exp);
            //Console.Write(m1.Name.ToString());
            //Console.Write("查询数量"+helper.QueryCount(exp));
            //更新
            //m.Name = "名称1";
            //helper.Update(m);
            //var exp1 = ExtLinq.True<mongoCustomer>().And(p => p.Name == "名称1");
            //var m1= helper.QueryByFirst(exp);
            //Console.Write(m.Name.ToString());

            //删除
            // helper.Delete(m);

            var UserModel = new users { Account = "名称", Tel = "123456", Address = "地址" };
            var m = new MongoDbHelper<users>().Insert(UserModel);
            Console.Read();
        } 
    }
}
