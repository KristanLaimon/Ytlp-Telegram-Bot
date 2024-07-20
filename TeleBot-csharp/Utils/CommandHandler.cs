using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot_csharp.BotUtils
{
    internal class CommandHandler
    {
        private Dictionary<string, ICommandModule> _allCommands = new();

        public void Add(ICommandModule newCommand)
        {
            _allCommands.Add(newCommand.Name, newCommand);
        }

        public async Task<bool> Run(TelegramBotClient bot, Message msg, string[] args, string commandName)
        {
            if (_allCommands.TryGetValue(commandName, out ICommandModule com))
            {
                try
                {
                    await com.ExecuteCommand(bot, msg, args);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }
    }
}
