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
                await teleBot.SendTextMessageAsync(
                    originalMsg.Chat, 
                    "Proporciona el link de youtube... e.g /download https://www.youtube.com/watch?v=uZeMMHDp4bs"
                );
                return;
            }

            await teleBot.SendTextMessageAsync(originalMsg.Chat, "Link de Youtube Reconocido...");
            VideoData metaData = await _ytdlp.DownloadOnlyInfo(args[0]);
            string formattedMeta = _ytdlp.FormatMetadata(metaData);
            var msg = await teleBot.SendTextMessageAsync(originalMsg.Chat, formattedMeta);
            await teleBot.EditMessageTextAsync(originalMsg.Chat, msg.MessageId, formattedMeta + "\nDescargando...");

            string size = "0 MB";
            string finalStringMsg = "";
            int holdupDelay = 0;
            var progressCallback = new Progress<DownloadProgress>(async p =>
            {
                holdupDelay++;
                if (p.ETA == null && holdupDelay % 20 != 0) return;

                string text =
                    "\n\n🦊 Descargado " + p.Progress * 100 + "%" +
                    "\n🚀 Velocidad de descarga: " + p.DownloadSpeed +
                    "\n⏳ ETA: " + p.ETA +
                    "\n💾 Size: " + p.TotalDownloadSize;
                finalStringMsg = formattedMeta + text;
                size = p.TotalDownloadSize;

                try
                {
                    await teleBot.EditMessageTextAsync(originalMsg.Chat, msg.MessageId, finalStringMsg);
                }
                catch
                {
                }
            });

            var downloadResult = await _ytdlp.DownloadVideo(args[0], metaData.Title, progressCallback);

            if (downloadResult.Success)
            {
                await teleBot.EditMessageTextAsync(originalMsg.Chat, msg.MessageId, finalStringMsg + "\n\nListo. Subiendo Video...");
                using (var stream = System.IO.File.OpenRead(downloadResult.Data))
                {
                    await teleBot.SendVideoAsync(originalMsg.Chat, stream, supportsStreaming: true);
                }
                System.IO.File.Delete(downloadResult.Data);
            }
            else
                await teleBot.SendTextMessageAsync(originalMsg.Chat, "No se ha podido descargar");
        }
    }
}
