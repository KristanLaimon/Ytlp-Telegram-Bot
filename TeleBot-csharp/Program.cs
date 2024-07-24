using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using DotNetEnv;
using TeleBot_csharp.commands;
using TeleBot_csharp.BotUtils;


#if DEBUG
    var bot = GenerateBotFromEnv();
#else
    TelegramBotClient bot;
    if(Environment.OSVersion.Platform == PlatformID.Win32NT)
        bot = GenerateBotFromEnv();
    else
    {
        //Is Unix System
        using var cts = new CancellationTokenSource();
        bot = 
            new TelegramBotClient(
                Environment.GetEnvironmentVariable("BOT_API_TOKEN") ?? 
                throw new Exception("You should provide a valid bot token to use this bot!"), cancellationToken: cts.Token
            );
    }
#endif

var me = await bot.GetMeAsync();
Console.WriteLine($"Bot Token Read\nInitiating @{me.Username}...");


await bot.LogOutAsync();


Console.WriteLine(bot.LocalBotServer);
//My dependencies - Handling Commands with Modules (Separated)
var commandHandler = new CommandHandler();
commandHandler.Add(new StartComm());
commandHandler.Add(new DownloadComm());
commandHandler.Add(new ButtonComm());

bot.OnMessage += AnyMessage;
bot.OnMessage += SlashCommand;

if (!Directory.Exists("downloads"))
    Directory.CreateDirectory("downloads");

Console.WriteLine("Bot Ready to go!");

//Keep running always until ctrl + c
var tcs = new TaskCompletionSource<bool>();
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true; 
    tcs.SetResult(true);
};
await tcs.Task;

#region Functions
TelegramBotClient GenerateBotFromEnv()
{
    Env.Load(Path.Join(Environment.CurrentDirectory, ".dev.env"));
    using var cts = new CancellationTokenSource();
    var bot = new TelegramBotClient(Env.GetString("BOT_API_TOKEN"), cancellationToken: cts.Token);
    return bot;
}
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
#endregion