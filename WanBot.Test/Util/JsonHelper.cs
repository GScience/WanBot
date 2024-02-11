using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Util;

namespace WanBot.Test.Util
{
    internal static class JsonHelper
    {
        public static void TestJsonSerialization<T>(string json) where T : class
        {
            if (typeof(T).IsAssignableTo(typeof(BaseMiraiEvent)))
                TestJsonSerializationEvent<T>(json);
            else if (typeof(T).IsAssignableTo(typeof(BaseChain)))
                TestJsonSerializationMessage<T>(json);
            else
                TestJsonSerializationEvent<T, object>(json);
        }

        public static void TestJsonSerializationEvent<T, TBase>(string json)
        {
            var obj
                = JsonSerializer.Deserialize<TBase>(json, MiraiJsonContext.Default.Options);
            var serializedJson = JsonSerializer.Serialize(obj, MiraiJsonContext.Default.Options);

            if (!IsJsonSame(serializedJson, json))
                Assert.Fail($"Failed while check {typeof(T).Name} protocol");
        }

        public static void TestJsonSerializationEvent<T>(string json)
        {
            TestJsonSerializationEvent<T, BaseMiraiEvent>(json);
        }

        public static void TestJsonSerializationMessage<T>(string json)
        {
            TestJsonSerializationEvent<T, BaseChain>(json);
        }

        public static bool IsJsonSame(string a, string b)
        {
            using var documentA = JsonDocument.Parse(a);
            using var documentB = JsonDocument.Parse(b);

            return IsJsonObjectSame(documentA.RootElement, documentB.RootElement);
        }

        private static bool IsJsonObjectSame(JsonElement a, JsonElement b)
        {
            var aDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(a);
            var bDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(b);

            if (aDict == null || bDict == null ||
                aDict.Count != bDict.Count)
                return false;

            foreach (var aPair in aDict)
            {
                if (!bDict.TryGetValue(aPair.Key, out var bValue))
                    return false;

                var aValue = aPair.Value;
                if (!IsJsonValueSame(aValue, bValue))
                    return false;
            }

            return true;
        }

        private static bool IsJsonValueSame(JsonElement aValue, JsonElement bValue)
        {
            if (aValue.ValueKind != bValue.ValueKind)
                return false;
            if (aValue.ValueKind == JsonValueKind.Object)
            {
                return IsJsonObjectSame(aValue, bValue);
            }
            else if (aValue.ValueKind == JsonValueKind.Array)
            {
                if (aValue.GetArrayLength() != bValue.GetArrayLength())
                    return false;
                var aCount = aValue.GetArrayLength();

                for (var i = 0; i < aCount; ++i)
                {
                    var aElement = aValue[i];
                    var bElement = bValue[i];

                    if (!IsJsonValueSame(aElement, bElement))
                        return false;
                }
            }
            else
            {
                if (Regex.Unescape(aValue.GetRawText()) != Regex.Unescape(bValue.GetRawText()))
                    return false;
            }

            return true;
        }
    }
}
