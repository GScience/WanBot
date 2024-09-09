using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Message
{
    public interface IMessageBuilder
    {
        void Text(string text);
        void Image(string imageUri);
        void Image(Span<byte> imageData);
        void Face(uint faceId);
        void Forward(IMessageChain chain);
        void MultiForward(IEnumerable<IMessageChain> chain);
        void Advance(uint messageType, string advanceMessage);
        IMessageChain Build();
    }
}
