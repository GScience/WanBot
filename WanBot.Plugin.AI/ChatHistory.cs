using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Plugin.AI
{
    public class ChatHistoryDictionary
    {
        private ConcurrentDictionary<long, GroupChatHistory> _chatDict = new();

        public void LogChat(long groupId, string chatStr, bool isBot)
        {
            if (!_chatDict.TryGetValue(groupId, out var chatHistory))
            {
                chatHistory = new GroupChatHistory();
                _chatDict[groupId] = chatHistory;
            }
            lock (chatHistory.Chats)
            {
                if (chatStr.Length > 50) chatStr = chatStr[..50];
                chatHistory.Chats.Add(new(chatStr, isBot));
                if (chatHistory.Chats.Count > 15)
                    chatHistory.Chats.RemoveAt(0);
            }
        }

        public GroupChat[] GetChatHistory(long groupId)
        {
            if (!_chatDict.TryGetValue(groupId, out var chatHistory))
                return Array.Empty<GroupChat>();
            lock (chatHistory.Chats)
            {
                return chatHistory.Chats.ToArray();
            }
        }
    }

    public record struct GroupChat(string Content, bool IsBotMessage);
    public class GroupChatHistory
    {
        public List<GroupChat> Chats = new();
    }
}
