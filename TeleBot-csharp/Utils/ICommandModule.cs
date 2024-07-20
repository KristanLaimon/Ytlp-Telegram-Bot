using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot_csharp.BotUtils
{
    internal interface ICommandModule
    {
        public string Name { get; }
        Task ExecuteCommand(TelegramBotClient bot, Message originalMsg, string[] args);
    }
}
