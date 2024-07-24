using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleBot_csharp.BotUtils;
using Telegram.Bot;
using Telegram.Bot.Types;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp;

namespace TeleBot_csharp.commands
{
    internal class DownloadComm : ICommandModule
    {
        private YtDownloader _ytdlp = new YtDownloader();

        public string Name => "download";

        public async Task ExecuteCommand(TelegramBotClient teleBot, Message originalMsg, string[] args)
        {
            if(args.Length == 0)
            {
                await teleBot.SendTextMessage(
                    originalMsg.Chat, 
                    "Proporciona el link de youtube... e.g /download https://www.youtube.com/watch?v="
                );
                return;
            }

            await teleBot.SendTextMessage(originalMsg.Chat, "Leyendo datos del video...");
            VideoData metaData = await _ytdlp.DownloadOnlyInfo(args[0]);
            string formattedMeta = _ytdlp.FormatMetadata(metaData);
            var msg = await teleBot.SendTextMessage(originalMsg.Chat, formattedMeta);
            await teleBot.EditMessageText(originalMsg.Chat, msg.MessageId, formattedMeta + "\nDescargando...");

            string size = "0 MB";
            string finalStringMsg = "";
            int holdupDelay = 0;
            var progressCallback = new Progress<DownloadProgress>(p =>
            {
                holdupDelay++;
                if (p.ETA == null && holdupDelay % 80 != 0) return;

                string text =
                    "\n\n🦊 Descargado " + p.Progress * 100 + "%" +
                    "\n🚀 Velocidad de descarga: " + p.DownloadSpeed +
                    "\n⏳ ETA: " + p.ETA +
                    "\n💾 Size: " + p.TotalDownloadSize;
                finalStringMsg = formattedMeta + text;
                size = p.TotalDownloadSize;

                teleBot.EditMessageText(originalMsg.Chat, msg.MessageId, finalStringMsg); ;
            });

            var downloadResult = await _ytdlp.DownloadVideo(args[0], metaData.Title, progressCallback);
            var thumbnailPath = metaData.Thumbnails[0].Url;

            if (downloadResult.Success)
            {
                await teleBot.EditMessageText(originalMsg.Chat, msg.MessageId, finalStringMsg + "\n\nListo. Subiendo Video...");
                using (var stream = System.IO.File.OpenRead(downloadResult.Data))
                {
                    if (originalMsg.MediaGroupId != null)
                        await teleBot.SendVideo(originalMsg.MediaGroupId, stream, supportsStreaming: true, thumbnail: thumbnailPath);
                    else
                        await teleBot.SendVideo(originalMsg.Chat, stream, supportsStreaming: true);
                }
                System.IO.File.Delete(downloadResult.Data);
            }
            else
                await teleBot.SendTextMessage(originalMsg.Chat, "No se ha podido descargar");
        }
    }
}
