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
    public class EventFriendSerializeTest
    {
        [TestMethod]
        public void TestFriendInputStatusChangedEventProtocol()
        {
            var json =
                """
                {
                  "type": "FriendInputStatusChangedEvent",
                  "friend": {
                    "id": 123123,
                    "nickname": "nick",
                    "remark": "remark"
                  }, 
                  "inputting": true
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<FriendInputStatusChangedEvent>(json);
        }

        [TestMethod]
        public void TestFriendNickChangedEventProtocol()
        {
            var json =
                """
                {
                  "type": "FriendNickChangedEvent",
                  "friend": {
                    "id": 123123,
                    "nickname": "nick",
                    "remark": "remark"
                  }, 
                  "from": "origin nickname",
                  "to": "new nickname"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<FriendNickChangedEvent>(json);
        }
    }
}
