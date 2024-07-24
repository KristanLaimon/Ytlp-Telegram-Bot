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
            var markup = new InlineKeyboardMarkup()
                .AddButton("--- Formats ----")
                .AddNewRow()
                .AddButton("240p", "240data");
            markup.InlineKeyboard.First().ToArray().ElementAt(0).

            await bot.SendTextMessage(originalMsg.Chat, "some Buttons", replyMarkup: markup);

                //InlineKeyboardButton.WithCallbackData()
        }
    }
}
