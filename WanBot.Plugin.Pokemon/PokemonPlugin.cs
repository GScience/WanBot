using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.Pokemon
{
    public class PokemonPlugin : WanBotPlugin
    {
        public override string PluginName => "PokemonPlugin";

        public override string PluginDescription => "查找生成随机宝可梦";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        /// <summary>
        /// 宝可梦数据库
        /// </summary>
        private PokemonDatabase? _pokeDatabase = null;

        public override void Init()
        {
            base.Init();
            _pokeDatabase = PokemonDatabase.Create(Logger, Path.Combine(GetConfigPath(), "poke.json"));
        }
        public override void Start()
        {
            this.GetBotHelp()
                .Category("宝可梦")
                .Command("#来只宝可梦", "随机抽一只宝可梦")
                .Command("#来只融合宝可梦", "随机抽一只???")
                .Command("#宝可梦 Id或名称", "查找宝可梦")
                .Command("#动态宝可梦 Id或名称", "查找宝可梦，并尝试发送动图")
                .Command("#融合宝可梦", "随机让两只宝可梦合体（？")
                .Info("融合出噩梦不要怪我x");

            base.Start();
        }

        [Command("宝可梦")]
        public async Task OnSearchPokemon(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "Search"))
                return;

            if (_pokeDatabase == null || _pokeDatabase.Pokemons.Length == 0)
                return;

            args.Blocked = true;

            // 查找模式
            var remain = args.GetRemain()?.ToArray();
            if (remain != null && remain.Length == 1 && remain[0] is Plain plain)
            {
                var searchKey = plain.Text;
                var pokemon = _pokeDatabase.Search(searchKey);
                if (pokemon != null)
                    await SendPokemon(args.Sender, pokemon);
                else
                    await args.Sender.ReplyAsync(new MessageBuilder().At(args.Sender).Text($"\n找不到宝可梦 {searchKey} 哦"));
                return;
            }
        }

        [Command("动态宝可梦")]
        public async Task OnDynamicPokemon(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "Search"))
                return;

            if (_pokeDatabase == null || _pokeDatabase.Pokemons.Length == 0)
                return;

            args.Blocked = true;

            // 查找模式
            var remain = args.GetRemain()?.ToArray();
            if (remain != null && remain.Length == 1 && remain[0] is Plain plain)
            {
                var searchKey = plain.Text;
                var pokemon = _pokeDatabase.Search(searchKey);
                if (pokemon != null)
                    await SendPokemon(args.Sender, pokemon, true);
                else
                    await args.Sender.ReplyAsync(new MessageBuilder().At(args.Sender).Text($"\n找不到宝可梦 {searchKey} 哦"));
                return;
            }
        }

        [Command("来只宝可梦")]
        public async Task OnRandomPokemon(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "Random"))
                return;

            if (_pokeDatabase == null || _pokeDatabase.Pokemons.Length == 0)
                return;

            args.Blocked = true;

            // 随机模式
            var randomPokemon = _pokeDatabase.Pokemons[new Random().Next(0, _pokeDatabase.Pokemons.Length)];
            await SendPokemon(args.Sender, randomPokemon);
        }

        [Command("来只融合宝可梦")]
        public async Task OnRandomFusionPokemon(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "Random"))
                return;

            if (!args.Sender.HasCommandPermission(this, "Fusion"))
                return;

            if (_pokeDatabase == null || _pokeDatabase.Pokemons.Length == 0)
                return;

            var fusionDir = Path.Combine(GetConfigPath(), "fusion");
            var dirs = Directory.GetDirectories(fusionDir);
            var dir = dirs[new Random().Next(0, dirs.Length)];

            var files = Directory.GetFiles(dir);
            var fullPath = files[new Random().Next(0, files.Length)];
            var fileNameArgs = Path.GetFileName(fullPath).Split('.');
            var idA = int.Parse(fileNameArgs[0]);
            var idB = int.Parse(fileNameArgs[1]);
            var pokemonA = _pokeDatabase.Search(idA.ToString("D4"));
            var pokemonB = _pokeDatabase.Search(idB.ToString("D4"));

            if (pokemonA == null)
            {
                await args.Sender.ReplyAsync($"随机到了漏洞");
                return;
            }
            if (pokemonB == null)
            {
                await args.Sender.ReplyAsync($"随机到了漏洞");
                return;
            }

            string newPokemonName;
            if (pokemonA == pokemonB)
                newPokemonName = pokemonA.Name[0] + pokemonB.Name;
            else
                newPokemonName = pokemonA.Name[0] + pokemonB.Name[1..];

            Logger.Info($"Fusion pokemon {fullPath}");

            if (System.IO.File.Exists(fullPath))
                await args.Sender.ReplyAsync(new MessageBuilder().Text($"你将 {pokemonA.Name} 和 {pokemonB.Name} 放到了一起，然后生出了 {newPokemonName} ({idA}.{idB})").ImageByPath(fullPath));
            else
                await args.Sender.ReplyAsync($"坏了，{newPokemonName}还没出生");
        }

        [Command("融合宝可梦")]
        public async Task OnFusionPokemon(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "Fusion"))
                return;

            if (_pokeDatabase == null || _pokeDatabase.Pokemons.Length == 0)
                return;

            args.Blocked = true;

            // 融合模式
            var codePlain = args.GetRemain()?.FirstOrDefault() as Plain;
            var cmdArgs = codePlain?.Text ?? "";

            var pokemonNames = cmdArgs.Split(' ');

            if (pokemonNames.Length != 2)
            {
                await args.Sender.ReplyAsync("啊啊啊你在融合什么！输入#融合宝可梦 宝可梦A名称 宝可梦B名称 来融合");
                return;
            }

            var pokemonA = _pokeDatabase.Search(pokemonNames[0]);
            var pokemonB = _pokeDatabase.Search(pokemonNames[1]);

            if (pokemonA == null)
            {
                await args.Sender.ReplyAsync($"{pokemonNames[0]} 是啥？");
                return;
            }
            if (pokemonB == null)
            {
                await args.Sender.ReplyAsync($"{pokemonNames[1]} 是啥？");
                return;
            }
            var topId = pokemonA.Id;
            var buttomId = pokemonB.Id;
            var fullPathWithoutExtension = Path.Combine(GetConfigPath(), "fusion", topId.ToString(), $"{topId}.{buttomId}");
            string newPokemonName;
            if (pokemonA == pokemonB)
                newPokemonName = pokemonA.Name[0] + pokemonB.Name;
            else
                newPokemonName = pokemonA.Name[0] + pokemonB.Name[1..];

            Logger.Info($"Fusion pokemon {fullPathWithoutExtension}");

            if (System.IO.File.Exists(fullPathWithoutExtension + ".gif"))
                await args.Sender.ReplyAsync(new MessageBuilder().Text($"{newPokemonName} ({topId}.{buttomId})").ImageByPath(fullPathWithoutExtension + ".gif"));
            else if (System.IO.File.Exists(fullPathWithoutExtension + ".png"))
                await args.Sender.ReplyAsync(new MessageBuilder().Text($"{newPokemonName} ({topId}.{buttomId})").ImageByPath(fullPathWithoutExtension + ".png"));
            else
                await args.Sender.ReplyAsync($"坏了，{newPokemonName}还没出生");
        }

        private async Task SendPokemon(ISender sender, PokemonElement pokemon, bool isDynamic = false)
        {
            if (!int.TryParse(pokemon.Id, out var id) ||
                id > 649)
                isDynamic = false;

            var url = isDynamic ?
                $"https://s1.52poke.wiki/assets/sprite/gen5/{id:D3}.gif" :
                pokemon.ImageUrl;

            var msg = new MessageBuilder()
                .At(sender)
                .Text("\n" + pokemon.DisplayName)
                .ImageByUrl($"{pokemon.ImageUrl}")
                .Text($"Id {pokemon.Id}");
                
            await sender.ReplyAsync(msg);
        }
    }
}