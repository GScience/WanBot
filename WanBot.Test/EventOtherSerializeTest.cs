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
    public class EventOtherSerializeTest
    {
        [TestMethod]
        public void TestOtherClientOnlineEventProtocol()
        {
            var json =
                """
                {
                  "type": "OtherClientOnlineEvent",
                  "client": {
                    "id": 1,
                    "platform": "WINDOWS"
                  },
                  "kind": 69899
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<OtherClientOnlineEvent>(json);
        }

        [TestMethod]
        public void TestOtherClientOfflineEventProtocol()
        {
            var json =
                """
                {
                  "type": "OtherClientOfflineEvent",
                  "client": {
                    "id": 1,
                    "platform": "WINDOWS"
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<OtherClientOfflineEvent>(json);
        }

        [TestMethod]
        public void TestCommandExecutedEventProtocol()
        {
            var json =
                """
                {
                  "type": "CommandExecutedEvent",
                  "name": "shutdown",
                  "friend": null,
                  "member": null,
                  "args": [
                    {
                      "type": "Plain",
                      "text": "myself"
                    }
                  ]
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<CommandExecutedEvent>(json);
        }
    }
}
