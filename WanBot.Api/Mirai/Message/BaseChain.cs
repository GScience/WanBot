using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Message
{
    [JsonDerivedType(typeof(App), nameof(App))]
    [JsonDerivedType(typeof(At), nameof(At))]
    [JsonDerivedType(typeof(AtAll), nameof(AtAll))]
    [JsonDerivedType(typeof(BaseChain), nameof(BaseChain))]
    [JsonDerivedType(typeof(Dice), nameof(Dice))]
    [JsonDerivedType(typeof(Face), nameof(Face))]
    [JsonDerivedType(typeof(File), nameof(File))]
    [JsonDerivedType(typeof(FlashImage), nameof(FlashImage))]
    [JsonDerivedType(typeof(Forward), nameof(Forward))]
    [JsonDerivedType(typeof(Image), nameof(Image))]
    [JsonDerivedType(typeof(Json), nameof(Json))]
    [JsonDerivedType(typeof(MarketFace), nameof(MarketFace))]
    [JsonDerivedType(typeof(MiraiCode), nameof(MiraiCode))]
    [JsonDerivedType(typeof(MusicShare), nameof(MusicShare))]
    [JsonDerivedType(typeof(Plain), nameof(Plain))]
    [JsonDerivedType(typeof(Poke), nameof(Poke))]
    [JsonDerivedType(typeof(Quote), nameof(Quote))]
    [JsonDerivedType(typeof(Source), nameof(Source))]
    [JsonDerivedType(typeof(Voice), nameof(Voice))]
    [JsonDerivedType(typeof(Xml), nameof(Xml))]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    public class BaseChain
    {
    }
}
