using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Message
{
    public record struct Group(uint Id)
    {
    }

    public record struct User(uint Id, Group Group)
    {
    }

    public record struct Text(string Str)
    {
    }
    public record struct Mention(User User, string Nickname)
    {
    }
    public record struct ImageUri(string Uri)
    {
    }
    public record struct Face(uint FaceId)
    {
    }
    public record struct Forward(IMessageChain Chain)
    {
    }
    public record struct MultiForward(IEnumerable<IMessageChain> Chain)
    {
    }
    public record struct Advance(uint MessageType, string AdvanceMessage)
    {
    }
    public interface IMessageChain
    {
        public int GetLength();
        public Type GetBlockType(int id);
        public T GetBlock<T>(int id);
    }
}
