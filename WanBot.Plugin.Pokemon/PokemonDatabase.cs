using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot.Plugin.Pokemon
{
    /// <summary>
    /// 宝可梦数据库
    /// </summary>
    internal class PokemonDatabase
    {
        public PokemonElement[] Pokemons = Array.Empty<PokemonElement>();

        public Dictionary<string, PokemonElement> NameIndex = new();
        public Dictionary<string, PokemonElement> IdIndex = new();

        private PokemonDatabase()
        {
        }

        public static PokemonDatabase Create(ILogger logger, string cachePath)
        {
            // 先尝试从缓存中加载
            var database = new PokemonDatabase();
            var jsonString = string.Empty;

            if (!File.Exists(cachePath))
            {
                logger.Info("Downloading pokemon database...");
                using var client = new HttpClient();
                var response = client.GetAsync("https://www.pokemon.cn/play/pokedex/api/v1?pokemon_ability_id=&zukan_id_from=1&zukan_id_to=1008").Result;
                jsonString = response.Content.ReadAsStringAsync().Result;
                File.WriteAllText(cachePath, jsonString);
                logger.Info("Finished");
            }
            else
                jsonString = File.ReadAllText(cachePath);
            var pokemonCollection = JsonSerializer.Deserialize<PokemonCollection>(jsonString);
            if (pokemonCollection != null)
                database.Pokemons = pokemonCollection.Pokemons;
            else
            {
                logger.Error("Failed to load pokemon database");
                return database;
            }

            // 构建索引
            foreach (var pokemon in database.Pokemons)
            {
                var nameKey = pokemon.Name + (pokemon.SubId == 0 ? "" : $"_{pokemon.SubId}");
                database.NameIndex[nameKey] = pokemon;
                string strId;
                if (pokemon.SubId == 0)
                    strId = pokemon.Id;
                else
                    strId = $"{pokemon.Id}_{pokemon.SubId}";
                database.IdIndex[strId] = pokemon;
            }

            return database;
        }

        public PokemonElement? Search(string key)
        {
            if (IdIndex.TryGetValue(key, out var idPokemon))
                return idPokemon;
            if (NameIndex.TryGetValue(key, out var namePokemon))
                return namePokemon;
            return null;
        }
    }

    public class PokemonCollection
    {
        [JsonPropertyName("pokemons")]
        public PokemonElement[] Pokemons { get; set; } = Array.Empty<PokemonElement>();
    }
    public class PokemonElement
    {
        /// <summary>
        /// 宝可梦Id
        /// 爬到的Id为string
        /// </summary>
        [JsonPropertyName("zukan_id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 宝可梦子Id
        /// </summary>
        [JsonPropertyName("zukan_sub_id")]
        public int SubId { get; set; }

        /// <summary>
        /// 宝可梦名称
        /// </summary>
        [JsonPropertyName("pokemon_name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 宝可梦子名称
        /// </summary>
        [JsonPropertyName("pokemon_sub_name")]
        public string SubName { get; set; } = string.Empty;

        /// <summary>
        /// 宝可梦图像名称
        /// </summary>
        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 宝可梦类型
        /// </summary>
        [JsonPropertyName("pokemon_type_name")]
        public string TypeName { get; set; } = string.Empty;

        private string _displayName = string.Empty;
        public string DisplayName
        {
            get
            {
                if (_displayName == string.Empty)
                {
                    // [属性1][属性2] 宝可梦名 (子名称)
                    _displayName = $"[{TypeName.Replace(",", "][")}] {Name}";
                    if (!string.IsNullOrEmpty(SubName))
                        _displayName += $" ({SubName})";
                }
                return _displayName;
            }
        }

        private string _imageUrl = string.Empty;
        public string ImageUrl
        {
            get
            {
                if (_imageUrl == string.Empty)
                    // [属性1][属性2] 宝可梦名 (子名称)
                    _imageUrl = $"https://www.pokemon.cn/play/resources/pokedex{FileName}";
                return _imageUrl;
            }
        }
    }
}
