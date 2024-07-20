using System.Text.RegularExpressions;

namespace TeleBot_csharp.BotUtils
{
    internal static class BotUtils
    {
        static public bool isYoutubeLink(string url)
        {
            //Mobile youtube link
            if (Regex.IsMatch(url, @"https:\/\/youtu\.be\/[\w|?=-]+"))
            {
                return true;
            }

            //Desktop youtube link
            if (Regex.IsMatch(url, "https:\\/\\/www\\.youtube\\.com\\/watch\\?v=[\\w|\\-]+"))
            {
                return true;
            }
            return false;
        }
        static public string SecondsToTime(int seconds)
        {
            if (seconds < 60)
            {
                return $"{seconds} Segundos";
            }
            else if (seconds < 3600)
            {
                int minutes = seconds / 60;
                seconds = seconds % 60;
                return $"{minutes}:{seconds} Minutos";
            }
            else
            {
                int hours = seconds / 3600;
                seconds = seconds % 3600;
                int minutes = seconds / 60;
                seconds = seconds % 60;
                return $"{hours}:{minutes}:{seconds} Horas";
            }
        }
        static public string NormalizeName(string name)
        {
            string toReturn =
                name.ToLower()
                .Replace(" ", "-")
                .Replace("#", "")
                .Replace("|", "");
            return toReturn;
        }
    }
}
