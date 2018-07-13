using System;
using System.Threading.Tasks;
using ServiceStack.Redis; 

namespace MyMongoDBTest.RedisHelper
{
    public class RedisPubSub
    {
        private static IRedisSubscription Subscription;
        /// <summary>
        /// 开始订阅频道
        /// </summary>
        /// <param name="name">频道名称</param>
        /// <param name="handler">接收到消息处理事件</param>
        public static void BeginSubscribe(string name, Action<string, string> handler)
        {
            if (Subscription != null)
                return;
            Task.Factory.StartNew(() => Start(name, handler));
        }
        public static void PublishMsg(string msg)
        {
            using (var client = RedisManager.GetClient())
            {
                client.PublishMessage("notice", msg);
            }
        }
        private static void Start(string name, Action<string, string> handleAction)
        {
            using (var client = RedisManager.GetClient())
            {
                //LogHelper.Info("创建频道连接",OpType.System);
                Subscription = client.CreateSubscription();
                //Subscription.OnUnSubscribe = channel =>
                //    LogHelper.Info("停止监听: " + channel, OpType.System);
                Subscription.OnMessage = (channel, msg) =>
                {
                    if (msg == "STOP")
                    {
                        //LogHelper.Info("断开所有监听频道...",OpType.System);
                        Subscription.UnSubscribeFromAllChannels(); //Un block thread.
                        return;
                    }
                    handleAction(channel, msg);
                };
                //LogHelper.Info("开始监听:" + name,OpType.System);
                Subscription.SubscribeToChannels(name);
            }
        }
    }
}
