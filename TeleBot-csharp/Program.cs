using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using DotNetEnv;
using TeleBot_csharp.commands;
using TeleBot_csharp.BotUtils;

Env.Load();

//Setup Bot
using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(Env.GetString("token"), cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");


//My dependencies - Handling Commands with Modules (Separated)
var commandHandler = new CommandHandler();
commandHandler.Add(new StartComm());
commandHandler.Add(new DownloadComm());


//Setup main methods for all this bot
bot.OnMessage += AnyMessage;
bot.OnMessage += SlashCommand;

//Set folder
if (!Directory.Exists("downloads"))
    Directory.CreateDirectory("downloads");

Console.ReadLine();
cts.Cancel();

async Task SlashCommand(Message message, UpdateType type)
{
    #region Verification
    if (message.Text is null) return;
    if (!message.Text.StartsWith("/")) return;
    var command = message.Text;

    var fullArgs = command.Remove(0, 1).Split(' ');
    var commandName = fullArgs[0].Replace("@" + me.Username, "");
    string[] args = fullArgs.Length > 1 ? fullArgs.Where((value, index) => index > 0).ToArray() : Array.Empty<string>();
    #endregion
    new Thread(async () => await commandHandler.Run(bot, message, args, commandName)).Start();
}
async Task AnyMessage(Message msg, UpdateType type)
{
    #region Verification
    if (msg.Text is null) return;
    if (msg.Text.StartsWith("/")) return;
    #endregion

    if (BotUtils.isYoutubeLink(msg.Text))
        new Thread(async () => await commandHandler.Run(bot, msg, [msg.Text], "download")).Start();
}
