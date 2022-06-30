using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Event;
using WanBot.Test.Util;

namespace WanBot.Test
{
    /// <summary>
    /// Bot事件事件序列化测试
    /// </summary>
    [TestClass]
    public class EventBotSerializeTest
    {
        [TestMethod]
        public void TestBotOnlineEventProtocol()
        {
            var json =
                """
                {
                  "type":"BotOnlineEvent",
                  "qq":123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotOnlineEvent>(json);
        }

        [TestMethod]
        public void TestBotOfflineEventActiveProtocol()
        {
            var json =
                """
                {
                  "type":"BotOfflineEventActive",
                  "qq":123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotOfflineEventActive>(json);
        }

        [TestMethod]
        public void TestBotOfflineEventForceProtocol()
        {
            var json =
                """
                {
                  "type":"BotOfflineEventForce",
                  "qq":123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotOfflineEventForce>(json);
        }

        [TestMethod]
        public void TestBotOfflineEventDroppedProtocol()
        {
            var json =
                """
                {
                  "type":"BotOfflineEventDropped",
                  "qq":123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotOfflineEventDropped>(json);
        }

        [TestMethod]
        public void TestBotReloginEventProtocol()
        {
            var json =
                """
                {
                  "type":"BotReloginEvent",
                  "qq":123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotReloginEvent>(json);
        }
    }
}
