using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace TeleBot_csharp.BotUtils
{
    internal class YtDownloader
    {
        private YoutubeDL _ytdl = new YoutubeDL();

        public YtDownloader()
        {
            _ytdl.OutputFolder = ".\\downloads";
            _ytdl.FFmpegPath = ".\\cli\\ffmpeg.exe";
            _ytdl.YoutubeDLPath = ".\\cli\\yt-dlp.exe";
        }

        /// <summary>
        /// Returns formatted info ad-hoc for this bot. In case of failure it returns a "" string (empty string)
        /// </summary>
        /// <param name="ytUrl">url from youtube</param>
        /// <param name="infoFormatted"></param>
        /// <returns>String with info. If Success all info, otherwise is empty ""</returns>
        public async Task<VideoData> DownloadOnlyInfo(string ytUrl)
        {
            var vidData = await _ytdl.RunVideoDataFetch(ytUrl);
            if (!vidData.Success) return new VideoData();
            return vidData.Data;
            
      
        }

        public string FormatMetadata(VideoData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("📽️ Título: " + data.Title);
            sb.AppendLine("📺 Canal: " + data.Channel);
            sb.AppendLine("📊 Seguidores: " + data.ChannelFollowerCount);
            sb.AppendLine("🧑‍💻 Uploader: " + data.Uploader);
            sb.AppendLine("⏱️ Duración: " + BotUtils.SecondsToTime((int)data.Duration));
            return sb.ToString();
        }

        /// <summary>
        /// Method with preconfigured parameters download to make that, download videos.
        /// </summary>
        /// <param name="ytUrl"></param>
        /// <param name="fileTitleNoExtension"></param>
        /// <param name="progressCallback"></param>
        /// <returns>Returns Path of the newly downloaded file</returns>
        public async Task<RunResult<string>> DownloadVideo(string ytUrl, string fileTitleNoExtension, Progress<DownloadProgress>? progressCallback )
        {
            var results =
                await _ytdl
                .RunVideoDownload(
                    ytUrl,
                    recodeFormat: VideoRecodeFormat.Mp4,
                    format: "b[filesize<100M] / w",
                    overrideOptions:
                        new OptionSet()
                        {
                            Output = Utils.Sanitize(fileTitleNoExtension) + ".mp4"
                        },
                    progress: progressCallback
            );
            return results;
        }

    }
}
