using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleBot_csharp.BotUtils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot_csharp.commands
{
    internal class StartComm : ICommandModule
    {
        public string Name => "start";

        public async Task ExecuteCommand(TelegramBotClient bot, Message originalMsg, string[] args)
        {
            await bot.SendTextMessage(originalMsg.Chat, "Hola!");
        }
    }
}
