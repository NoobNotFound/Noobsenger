using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noobsenger.Core;

namespace Noobsenger.MAUI.Helpers;

public static class Extentions
{
    public static bool IsNullEmptyOrWhiteSpace(this string str) =>
        string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    public static string ToImageName(this Avatars avatar) => avatar switch
    {
        Avatars.Boy => "boy",
        Avatars.Gamer => "gamer",
        Avatars.Girl => "girl",
        Avatars.Man => "man",
        Avatars.Man2 => "man_two",
        Avatars.Man3 => "man_three",
        Avatars.MaskedMan => "masked_man",
        Avatars.Nerd => "nerd",
        Avatars.Sir => "sir",
        Avatars.Woman => "woman",
        Avatars.Woman1 => "woman_two",
        Avatars.Woman2 => "woman_three",
        _ => "open_ai"
    } + ".png";

}