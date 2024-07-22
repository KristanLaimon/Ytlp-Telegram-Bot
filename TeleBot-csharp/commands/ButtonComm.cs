using TeleBot_csharp.BotUtils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot_csharp.commands
{
    internal class ButtonComm : ICommandModule
    {
        public string Name => "button";

        public async Task ExecuteCommand(TelegramBotClient bot, Message originalMsg, string[] args)
        {
            var replyMarkup = new ReplyKeyboardMarkup(true)
                .AddButton("--- Video ---")
                .AddNewRow()
                .AddButtons("Option1", "Option2");

                //InlineKeyboardButton.WithCallbackData()
        }
    }
}
