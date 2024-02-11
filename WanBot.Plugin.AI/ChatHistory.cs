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

        public void LogChat(long groupId, string chatStr)
        {
            if (!_chatDict.TryGetValue(groupId, out var chatHistory))
            {
                chatHistory = new GroupChatHistory();
                _chatDict[groupId] = chatHistory;
            }
            lock (chatHistory.Chats)
            {
                chatHistory.Chats.Add(chatStr);
                if (chatHistory.Chats.Count > 20)
                    chatHistory.Chats.RemoveAt(0);
            }
        }

        public string GetFormatedChatHistory(long groupId, char lineSplit, int maxLength)
        {
            if (!_chatDict.TryGetValue(groupId, out var chatHistory))
                return "";
            var result = "";
            lock (chatHistory.Chats)
            {
                foreach (var chat in chatHistory.Chats)
                {
                    if (chat.Length > maxLength)
                        result += chat[..maxLength] + lineSplit;
                    else
                        result += chat + lineSplit;
                }
            }
            return result;
        }
    }

    public class GroupChatHistory
    {
        public List<string> Chats = new();
    }
}
