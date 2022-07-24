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
    /// 消息事件事件序列化测试
    /// </summary>
    [TestClass]
    public class EventMessageSerializeTest
    {
        [TestMethod]
        public void TestFriendMessageProtocol()
        {
            var json =
                """
                {
                    "type": "FriendMessage",
                    "sender": {
                        "id": 123,
                        "nickname": "",
                        "remark": ""
                    },
                    "messageChain": []
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<FriendMessage>(json);
        }

        [TestMethod]
        public void TestGroupMessageProtocol()
        {
            var json =
                """
                {
                    "type": "GroupMessage",
                    "sender": {
                        "id": 123,
                        "memberName": "",
                        "specialTitle": "",
                        "permission": "OWNER",
                        "joinTimestamp": 0,
                        "lastSpeakTimestamp": 0,
                        "muteTimeRemaining": 0,
                        "group": {
                            "id": 321,
                            "name": "",
                            "permission": "MEMBER"
                        }
                    },
                    "messageChain": []
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<GroupMessage>(json);
        }

        [TestMethod]
        public void TestTempMessageProtocol()
        {
            var json =
                """
                {
                    "type": "TempMessage",
                    "sender": {
                        "id": 123,
                        "memberName": "",
                        "specialTitle": "",
                        "permission": "OWNER",
                        "joinTimestamp": 0,
                        "lastSpeakTimestamp": 0,
                        "muteTimeRemaining": 0,
                        "group": {
                            "id": 321,
                            "name": "",
                            "permission": "MEMBER"
                        }
                    },
                    "messageChain": []
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<TempMessage>(json);
        }

        [TestMethod]
        public void TestStrangerMessageProtocol()
        {
            var json =
                """
                {
                    "type": "StrangerMessage",
                    "sender": {
                        "id": 123,
                        "nickname": "",
                        "remark": ""
                    },
                    "messageChain": []
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<StrangerMessage>(json);
        }

        [TestMethod]
        public void TestOtherClientMessageProtocol()
        {
            var json =
                """
                {
                    "type": "OtherClientMessage",
                    "sender": {
                        "id": 123,
                        "platform": "MOBILE"
                    },
                    "messageChain": []
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<OtherClientMessage>(json);
        }
    }
}
