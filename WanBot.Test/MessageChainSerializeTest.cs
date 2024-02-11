using System.Diagnostics;
using System.Text.Json;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Util;
using WanBot.Test.Util;

namespace WanBot.Test
{
    /// <summary>
    /// 消息链序列化测试
    /// </summary>
    [TestClass]
    public class MessageChainSerializeTest
    {
        /// <summary>
        /// 测试序列化后反序列化，并检查结果是否正常
        /// </summary>
        [TestMethod]
        public void TestMessageChainDeserialize()
        {
            var typesInAsm = typeof(BaseChain).Assembly.GetTypes();
            var messageTypes = typesInAsm.Where(
                t => typeof(BaseChain).IsAssignableFrom(t) && !t.IsAbstract
                );

            Debug.WriteLine("Create MessageChain for testing");
            var messages = new List<BaseChain>();
            foreach (var type in messageTypes)
                messages.Add((Activator.CreateInstance(type) as BaseChain)!);
            var chain = new MessageChain(messages);

            Debug.WriteLine("Try to serialize");
            var json = JsonSerializer.Serialize(chain, MiraiJsonContext.Default.MessageChain);

            Debug.WriteLine(json);

            Debug.WriteLine("Try to deserialize");
            var messageChain = JsonSerializer.Deserialize(json, MiraiJsonContext.Default.MessageChain);
            if (messageChain == null)
                Assert.Fail("Failed to deserialize message");

            foreach (var message in messageChain)
                if (message == null)
                    Assert.Fail("Failed to deserialize message");
        }

        [TestMethod]
        public void TestForwardDeserialize()
        {
            string testString = "Hello World";

            Debug.WriteLine("Create MessageChain for testing");
            var forwardMessage = new Forward();
            var nodeList = new List<Forward.ForwardNode>();
            forwardMessage.NodeList = nodeList;
            nodeList.Add(new Forward.ForwardNode()
            {
                MessageChain = new MessageChain(
                    new BaseChain[]
                    {
                        new Image(),
                        new Plain()
                        {
                            Text = testString
                        }
                    })
            });
            var chain = new MessageChain(new BaseChain[] { forwardMessage });

            Debug.WriteLine("Try to deserialize");
            var json = JsonSerializer.Serialize(chain, MiraiJsonContext.Default.MessageChain);

            Debug.WriteLine(json);
            var messageChain = JsonSerializer.Deserialize(json, MiraiJsonContext.Default.MessageChain);

            if (messageChain.First() is not Forward serializedForward)
            {
                Assert.Fail("Failed to deserialize message");
                throw new Exception();
            }

            if (serializedForward.NodeList![0].MessageChain!.ElementAt(1) is not Plain serlializedPlain)
            {
                Assert.Fail("Failed to deserialize message");
                throw new Exception();
            }

            if (serlializedPlain.Text != testString)
                Assert.Fail("Failed to deserialize message");
        }

        [TestMethod]
        public void TestSourceProtocol()
        {
            var json =
                """
                {
                    "type": "Source",
                    "id": 123456,
                    "time": 123456
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Source>(json);
        }

        [TestMethod]
        public void TestQuoteProtocol()
        {
            var json =
                """
                {
                    "type": "Quote",
                    "id": 123456,
                    "groupId": 123456789,
                    "senderId": 987654321,
                    "targetId": 9876543210,
                    "origin": [
                        { "type": "Plain", "text": "text" }
                    ] 
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Quote>(json);
        }

        [TestMethod]
        public void TestAtProtocol()
        {
            var json =
                """
                {
                    "type": "At",
                    "target": 123456,
                    "display": "@Mirai"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<At>(json);
        }

        [TestMethod]
        public void TestAtAllProtocol()
        {
            var json =
                """
                {
                    "type": "AtAll"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<AtAll>(json);
        }

        [TestMethod]
        public void TestFaceProtocol()
        {
            var json =
                """
                {
                    "type": "Face",
                    "faceId": 123,
                    "name": "bu"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Face>(json);
        }

        [TestMethod]
        public void TestPlainProtocol()
        {
            var json =
                """
                {
                    "type": "Plain",
                    "text": "Mirai"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Plain>(json);
        }

        [TestMethod]
        public void TestImageProtocol()
        {
            var json =
                """
                {
                    "type": "Image",
                    "imageId": "{01E9451B-70ED-EAE3-B37C-101F1EEBF5B5}.mirai",
                    "url": "https://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    "path": null,
                    "base64": null
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Image>(json);
        }

        [TestMethod]
        public void TestFlashImageProtocol()
        {
            var json =
                """
                {
                    "type": "FlashImage",
                    "imageId": "/f8f1ab55-bf8e-4236-b55e-955848d7069f",
                    "url": "https://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    "path": null,
                    "base64": null
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<FlashImage>(json);
        }

        [TestMethod]
        public void TestVoiceProtocol()
        {
            var json =
                """
                {
                    "type": "Voice",
                    "voiceId": "23C477720A37FEB6A9EE4BCCF654014F.amr",
                    "url": "https://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    "path": null,
                    "base64": null,
                    "length": 1024
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Voice>(json);
        }

        [TestMethod]
        public void TestXmlProtocol()
        {
            var json =
                """
                {
                    "type": "Xml",
                    "xml": "XML"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Xml>(json);
        }

        [TestMethod]
        public void TestJsonProtocol()
        {
            var json =
                """
                {
                    "type": "Json",
                    "json": "JSON"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Json>(json);
        }

        [TestMethod]
        public void TestAppProtocol()
        {
            var json =
                """
                {
                    "type": "App",
                    "content": "APP"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<App>(json);
        }

        [TestMethod]
        public void TestPokeProtocol()
        {
            var json =
                """
                {
                    "type": "Poke",
                    "name": "SixSixSix"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Poke>(json);
        }

        [TestMethod]
        public void TestDiceProtocol()
        {
            var json =
                """
                {
                  "type": "Dice",
                  "value": 1
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Dice>(json);
        }

        [TestMethod]
        public void TestMarketFaceProtocol()
        {
            var json =
                """
                {
                  "type": "MarketFace",
                  "id": 123,
                  "name": "MARKET FACE"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<MarketFace>(json);
        }

        [TestMethod]
        public void TestMusicShareProtocol()
        {
            var json =
                """
                {
                  "type": "MusicShare",
                  "kind": "String",
                  "title": "String",
                  "summary": "String",
                  "jumpUrl": "String",
                  "pictureUrl": "String",
                  "musicUrl": "String",
                  "brief": "String"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<MusicShare>(json);
        }

        [TestMethod]
        public void TestForwardMessageProtocol()
        {
            var json =
                """
                {
                  "type": "Forward",
                  "nodeList": [
                    {
                      "senderId": 123,
                      "time": 0,
                      "senderName": "sender name",
                      "messageChain": [],
                      "messageId": 123
                    }
                  ] 
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Forward>(json);
        }

        [TestMethod]
        public void TestFileProtocol()
        {
            var json =
                """
                {
                  "type": "File",
                  "id": "",
                  "name": "",
                  "size": 0
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<Api.Mirai.Message.File>(json);
        }

        [TestMethod]
        public void TestMiraiCodeProtocol()
        {
            var json =
                """
                {
                  "type": "MiraiCode",
                  "code": "hello[mirai:at:1234567]"
                }
                """;
            JsonHelper.TestJsonSerialization<BaseChain>(json);
            JsonHelper.TestJsonSerialization<MiraiCode>(json);
        }
    }
}