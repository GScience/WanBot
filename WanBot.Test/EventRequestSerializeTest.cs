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
    /// 申请事件事件序列化测试
    /// </summary>
    [TestClass]
    public class EventRequestSerializeTest
    {
        [TestMethod]
        public void TestNewFriendRequestEventProtocol()
        {
            var json =
                """
                {
                  "type": "NewFriendRequestEvent",
                  "eventId": 12345678,
                  "fromId": 123456,
                  "groupId": 654321,
                  "nick": "Nick Name",
                  "message": ""
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<NewFriendRequestEvent>(json);
        }

        [TestMethod]
        public void TestMemberJoinRequestEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberJoinRequestEvent",
                  "eventId": 12345678,
                  "fromId": 123456,
                  "groupId": 654321,
                  "groupName": "Group",
                  "nick": "Nick Name",
                  "message": ""
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<MemberJoinRequestEvent>(json);
        }

        [TestMethod]
        public void TestBotInvitedJoinGroupRequestEventProtocol()
        {
            var json =
                """
                {
                  "type": "BotInvitedJoinGroupRequestEvent",
                  "eventId": 12345678,
                  "fromId": 123456,
                  "groupId": 654321,
                  "groupName": "Group",
                  "nick": "Nick Name",
                  "message": ""
                }
                """;
            JsonHelper.TestJsonSerialization<BaseMiraiEvent>(json);
            JsonHelper.TestJsonSerialization<BotInvitedJoinGroupRequestEvent>(json);
        }
    }
}
