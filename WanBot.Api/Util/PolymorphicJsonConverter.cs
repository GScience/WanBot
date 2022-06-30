using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Util
{
    /// <summary>
    /// 可多态反序列化对象
    /// </summary>
    public interface ISerializablePolymorphic
    {
        string Type { get; }
    }

    /// <summary>
    /// Json反序列化处理
    /// </summary>
    internal class PolymorphicJsonConverter<T> : JsonConverter<T> where T : class, ISerializablePolymorphic
    {
        private const string TypeDiscriminatorPropertyName = "type";

        /// <summary>
        /// 消息类型字典
        /// </summary>
        private readonly Dictionary<string, Type> _typeDict = new();

        public PolymorphicJsonConverter()
        {
            var typesInAsm = GetType().Assembly.GetTypes();
            var messageTypes = typesInAsm.Where(
                t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract
                );
            foreach (var type in messageTypes)
                _typeDict.Add(type.Name, type);
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(ISerializablePolymorphic).IsAssignableFrom(typeToConvert);

        public override T Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string? propertyName = readerClone.GetString();
            if (propertyName != TypeDiscriminatorPropertyName)
                throw new JsonException();

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.String)
                throw new JsonException();

            var typeName = readerClone.GetString() ?? throw new JsonException();
            if (!_typeDict.TryGetValue(typeName, out var type))
                throw new JsonException();

            var baseChain = JsonSerializer.Deserialize(ref reader, type, options) as T;
            return baseChain!;
        }

        public override void Write(
            Utf8JsonWriter writer, T obj, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, obj, obj.GetType(), options);
        }
    }
}
