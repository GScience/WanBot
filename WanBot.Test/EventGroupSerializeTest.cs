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
    public class EventGroupSerializeTest
    {
        [TestMethod]
        public void TestBotGroupPermissionChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "BotGroupPermissionChangeEvent",
                  "origin": "MEMBER",
                  "current": "ADMINISTRATOR",
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "ADMINISTRATOR"
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotGroupPermissionChangeEvent>(json);
        }

        [TestMethod]
        public void TestBotMuteEventProtocol()
        {
            var json =
                """
                {
                  "type": "BotMuteEvent",
                  "durationSeconds": 600,
                  "operator": {
                    "id": 123456789,
                    "memberName": "IAMADMIN",
                    "permission": "ADMINISTRATOR",
                    "specialTitle":"SpecialTitle",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotMuteEvent>(json);
        }

        [TestMethod]
        public void TestBotUnmuteEventProtocol()
        {
            var json =
                """
                {
                  "type": "BotUnmuteEvent",
                  "operator": {
                    "id": 123456789,
                    "memberName": "IAMADMIN",
                    "permission": "ADMINISTRATOR",
                    "specialTitle":"SpecialTitle",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotUnmuteEvent>(json);
        }

        [TestMethod]
        public void TestBotJoinGroupEventProtocol()
        {
            var json =
                """
                {
                  "type": "BotJoinGroupEvent",
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "invitor": null
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotJoinGroupEvent>(json);
        }

        [TestMethod]
        public void TestBotLeaveEventActiveProtocol()
        {
            var json =
                """
                {
                  "type": "BotLeaveEventActive",
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotLeaveEventActive>(json);
        }

        [TestMethod]
        public void TestBotLeaveEventKickProtocol()
        {
            var json =
                """
                {
                  "type": "BotLeaveEventKick",
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "operator": null
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<BotLeaveEventKick>(json);
        }

        [TestMethod]
        public void TestGroupRecallEventProtocol()
        {
            var json =
                """
                {
                   "type": "GroupRecallEvent",
                   "authorId": 123456,
                   "messageId": 123456789,
                   "time": 1234679,
                   "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "ADMINISTRATOR"
                   },
                   "operator": {
                      "id": 123456789,
                      "memberName": "IAMADMIN",
                      "permission": "ADMINISTRATOR",
                      "specialTitle":"SpecialTitle",
                      "joinTimestamp":12345678,
                      "lastSpeakTimestamp":8765432,
                      "muteTimeRemaining":0,
                      "group": {
                        "id": 123456789,
                        "name": "Miral Technology",
                        "permission": "MEMBER"
                      }
                   }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupRecallEvent>(json);
        }

        [TestMethod]
        public void TestFriendRecallEventProtocol()
        {
            var json =
                """
                {
                    "type": "FriendRecallEvent",
                    "authorId": 123456,
                    "messageId": 123456789,
                    "time": 1234679,
                    "operator": 123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<FriendRecallEvent>(json);
        }

        [TestMethod]
        public void TestNudgeEventProtocol()
        {
            var json =
                """
                {
                    "type": "NudgeEvent",
                    "fromId": 123456,
                    "subject": {
                        "id": 123456,
                        "kind": "Group"
                    },
                    "action": "Nudge",
                    "suffix": "Face",
                    "target": 123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<NudgeEvent>(json);
        }

        [TestMethod]
        public void TestGroupNameChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "GroupNameChangeEvent",
                  "origin": "miral technology",
                  "current": "MIRAI TECHNOLOGY",
                  "group": {
                    "id": 123456789,
                    "name": "MIRAI TECHNOLOGY",
                    "permission": "MEMBER"
                  },
                  "operator": {
                    "id": 123456,
                    "memberName": "我是群主",
                    "permission": "ADMINISTRATOR",
                    "specialTitle":"群头衔",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "OWNER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupNameChangeEvent>(json);
        }

        [TestMethod]
        public void TestGroupEntranceAnnouncementChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "GroupEntranceAnnouncementChangeEvent",
                  "origin": "abc",
                  "current": "cba",
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "operator": {
                    "id": 123456789,
                    "memberName": "我是管理员",
                    "permission": "ADMINISTRATOR",
                    "specialTitle":"群头衔",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupEntranceAnnouncementChangeEvent>(json);
        }

        [TestMethod]
        public void TestGroupMuteAllEventProtocol()
        {
            var json =
                """
                {
                  "type": "GroupMuteAllEvent",
                  "origin": false,
                  "current": true,
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "operator": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"OWNER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupMuteAllEvent>(json);
        }

        [TestMethod]
        public void TestGroupAllowAnonymousChatEventProtocol()
        {
            var json =
                """
                {
                  "type": "GroupAllowAnonymousChatEvent",
                  "origin": false,
                  "current": true,
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "operator": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"OWNER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupAllowAnonymousChatEvent>(json);
        }

        [TestMethod]
        public void TestGroupAllowConfessTalkEventProtocol()
        {
            var json =
                """
                {
                  "type": "GroupAllowConfessTalkEvent",
                  "origin": false,
                  "current": true,
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "isByBot": false
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupAllowConfessTalkEvent>(json);
        }

        [TestMethod]
        public void TestGroupAllowMemberInviteEventProtocol()
        {
            var json =
                """
                {
                  "type": "GroupAllowMemberInviteEvent",
                  "origin": false,
                  "current": true,
                  "group": {
                    "id": 123456789,
                    "name": "Miral Technology",
                    "permission": "MEMBER"
                  },
                  "operator": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"OWNER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<GroupAllowMemberInviteEvent>(json);
        }

        [TestMethod]
        public void TestMemberJoinEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberJoinEvent",
                  "member": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"MEMBER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  },
                  "invitor": null
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberJoinEvent>(json);
        }

        [TestMethod]
        public void TestMemberLeaveEventKickProtocol()
        {
            var json =
                """
                {
                  "type": "MemberLeaveEventKick",
                  "member": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"MEMBER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  },
                  "operator": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"OWNER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberLeaveEventKick>(json);
        }

        [TestMethod]
        public void TestMemberLeaveEventQuitProtocol()
        {
            var json =
                """
                {
                  "type": "MemberLeaveEventQuit",
                  "member": {
                    "id": 123456789,
                    "memberName": "我是被踢的",
                    "permission": "MEMBER",
                    "specialTitle":"群头衔",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberLeaveEventQuit>(json);
        }

        [TestMethod]
        public void TestMemberCardChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberCardChangeEvent",
                  "origin": "origin name",
                  "current": "我是被改名的",
                  "member": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"MEMBER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }  
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberCardChangeEvent>(json);
        }

        [TestMethod]
        public void TestMemberSpecialTitleChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberSpecialTitleChangeEvent",
                  "origin": "origin title",
                  "current": "new title",
                  "member": {
                    "id": 123456789,
                    "memberName": "我是被改头衔的",
                    "permission": "MEMBER",
                    "specialTitle":"群头衔",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberSpecialTitleChangeEvent>(json);
        }

        [TestMethod]
        public void TestMemberPermissionChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberPermissionChangeEvent",
                  "origin": "MEMBER",
                  "current": "ADMINISTRATOR",
                  "member": {
                    "id": 123456789,
                    "memberName": "我是被改权限的",
                    "specialTitle":"群头衔",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "permission": "ADMINISTRATOR",
                    "group": {
                      "id": 123456789,
                      "name": "Miral Technology",
                      "permission": "MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberPermissionChangeEvent>(json);
        }

        [TestMethod]
        public void TestMemberMuteEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberMuteEvent",
                  "durationSeconds": 600,
                  "member": {
                    "id":1234567890,
                    "memberName":"我是被取消禁言的",
                    "specialTitle":"群头衔",
                    "permission":"MEMBER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  },
                  "operator": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"OWNER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberMuteEvent>(json);
        }

        [TestMethod]
        public void TestMemberUnmuteEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberUnmuteEvent",
                  "member": {
                    "id":1234567890,
                    "memberName":"我是被取消禁言的",
                    "specialTitle":"群头衔",
                    "permission":"MEMBER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  },
                  "operator": {
                    "id":1234567890,
                    "memberName":"",
                    "specialTitle":"群头衔",
                    "permission":"OWNER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  }
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberUnmuteEvent>(json);
        }

        [TestMethod]
        public void TestMemberHonorChangeEventProtocol()
        {
            var json =
                """
                {
                  "type": "MemberHonorChangeEvent",
                  "member": {
                    "id":1234567890,
                    "memberName":"我是被取消禁言的",
                    "specialTitle":"群头衔",
                    "permission":"MEMBER",
                    "joinTimestamp":12345678,
                    "lastSpeakTimestamp":8765432,
                    "muteTimeRemaining":0,
                    "group":{
                      "id":12345,
                      "name":"群名1",
                      "permission":"MEMBER"
                    }
                  },
                  "action": "achieve",
                  "honor": "龙王"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseEvent>(json);
            JsonHelper.TestJsonSerialization<MemberHonorChangeEvent>(json);
        }
    }
}
