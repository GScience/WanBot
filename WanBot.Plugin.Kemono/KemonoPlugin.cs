using System.Net.Http.Json;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.Kemono
{
    public class KemonoPlugin : WanBotPlugin
    {
        public const string ApiHost = "https://kemono.su";
        public const string ApiUrl = $"{ApiHost}/api/v1";

        public override string PluginName => "Kemono";

        public override string PluginDescription => "奇怪的小插件";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private CreatorInfo[] _creators = Array.Empty<CreatorInfo>();

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public override void PreInit()
        {
            base.PreInit();
            this.GetBotHelp()
                .Category("奇怪小功能")
                .Command("#查作者 <作者名> [服务]", "查询你想要的作者")
                .Command("#查内容 <作者名> <查询内容> [服务]", "查看他发了什么")
                .Command("#查最新 <作者名> [服务]", "查看他最新发了什么");
            RefreshAsync().Wait();
        }

        private CreatorInfo SearchCreator(string name, string service = "")
        {
            name = name.ToLower();
            service ??= "";
            service = service.ToLower();
            foreach (var creator in _creators)
                if (creator.Service.ToLower().StartsWith(service) &&
                    creator.Name.ToLower().StartsWith(name))
                    return creator;
            return new CreatorInfo();
        }

        private async Task<PostInfo[]> GetPostAsync(CreatorInfo creatorInfo)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{ApiUrl}/{creatorInfo.Service}/user/{creatorInfo.Id}");
            var postArray = await response.Content.ReadFromJsonAsync<PostInfo[]>();
            if (postArray == null) return Array.Empty<PostInfo>();
            return postArray;
        }

        private async Task RefreshAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{ApiUrl}/creators.txt");
            var creatorArray = await response.Content.ReadFromJsonAsync<CreatorInfo[]>();
            if (creatorArray == null)
                Logger.Error("Failed to get creators.txt");
            else
                _creators = creatorArray;
        }

        [Command("查作者")]
        public async Task OnSearchCreatorCommand(MiraiBot bot, CommandEventArgs args)
        {
            args.Blocked = true;
            if (!_semaphore.Wait(0))
            {
                await args.Sender.ReplyAsync($"别问了，歇会儿", args.GetMessageId());
                return;
            }
            try
            {
                var createrKeyword = args.GetNextArgs<string>();
                var serverKeyword = args.GetNextArgs<string>();
                var creator = SearchCreator(createrKeyword, serverKeyword);
                await args.Sender.ReplyAsync($"猜你想找：{creator.Name}", args.GetMessageId());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        [Command("查内容")]
        public async Task OnSearchPostCommand(MiraiBot bot, CommandEventArgs args)
        {
            args.Blocked = true;
            if (!_semaphore.Wait(0))
            {
                await args.Sender.ReplyAsync($"别问了，歇会儿", args.GetMessageId());
                return;
            }
            try
            {
                var createrKeyword = args.GetNextArgs<string>();
                var postKeyword = args.GetNextArgs<string>();
                var serverKeyword = args.GetNextArgs<string>();
                var creator = SearchCreator(createrKeyword, serverKeyword);
                var posts = await GetPostAsync(creator);
                if (posts.Length == 0)
                {
                    await args.Sender.ReplyAsync($"坏了，啥也没找到");
                    return;
                }
                foreach (var post in posts)
                {
                    if (post.Title.Contains(serverKeyword) ||
                        post.Content.Contains(serverKeyword))
                    {
                        await SendPostAsync(args.Sender, posts[0], args.GetMessageId());
                        return;
                    }
                }
                await args.Sender.ReplyAsync($"坏了，没找到");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        [Command("查最新")]
        public async Task OnSearchNewestCommand(MiraiBot bot, CommandEventArgs args)
        {
            args.Blocked = true;
            if (!_semaphore.Wait(0))
            {
                await args.Sender.ReplyAsync($"别问了，歇会儿", args.GetMessageId());
                return;
            }
            try
            {
                if (!args.Sender.HasPermission(this, "Search")) return;
                var createrKeyword = args.GetNextArgs<string>();
                var serverKeyword = args.GetNextArgs<string>();
                var creator = SearchCreator(createrKeyword, serverKeyword);
                if (string.IsNullOrEmpty(creator.Name))
                {
                    await args.Sender.ReplyAsync($"坏了，不认识这个人", args.GetMessageId());
                    return;
                }
                var posts = await GetPostAsync(creator);
                if (posts.Length == 0)
                {
                    await args.Sender.ReplyAsync($"坏了，啥也没找到", args.GetMessageId());
                    return;
                }
                await SendPostAsync(args.Sender, posts[0], args.GetMessageId());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SendPostAsync(ISender sender, PostInfo post, int? replyId = null)
        {
            var formatedMessage = $"{post.Title}\n{post.Content}\n附件：";
            foreach (var att in post.Attachments)
                formatedMessage += $"\n[{att.Name}]{ApiHost}{att.Path}";
            await sender.ReplyAsImageAsync(formatedMessage, replyId);
        }
    }

    public record struct CreatorInfo(
        string Id,
        string Name,
        string Service,
        long Indexed,
        long Updated,
        long Favorited);

    public record struct PostInfo(
        string Id,
        string User,
        string Service,
        string Title,
        string Content,
        PostFileInfo File,
        PostFileInfo[] Attachments);

    public record struct PostFileInfo(
        string Name,
        string Path);
}
