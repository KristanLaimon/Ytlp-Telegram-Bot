global using TelegramBotClient = WTelegram.Bot;
global using Update = WTelegram.Types.Update;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using DotNetEnv;
using TeleBot_csharp.commands;
using TeleBot_csharp.BotUtils;
using System.Data.SQLite;
using TL.Methods;

TelegramBotClient bot;

#if DEBUG
    Env.Load(Path.Join(Environment.CurrentDirectory, ".dev.env"));
    bot = GenerateBotFromEnvVariables();
#else
    if(Environment.OSVersion.Platform == PlatformID.Win32NT)
        Env.Load(Path.Join(Environment.CurrentDirectory, ".dev.env"));

    bot = GenerateBotFromEnvVariables();
#endif

TelegramBotClient GenerateBotFromEnvVariables()
{
    using var cts = new CancellationTokenSource();

    var dbPath = Path.Join(Environment.CurrentDirectory, "db", "ytfox.db");
    var dbConn = new SQLiteConnection($"Data Source={dbPath};Version=3;");

    string bot_token = Env.GetString("API_BOT_TOKEN") ?? 
        throw new Exception("You should provide API_BOT_TOKEN Environment Variable");

    var api_id = Env.GetInt("API_ID");
    if(api_id == 0)
        throw new Exception("You should provide now API_ID environment variable");

    var api_hash = Env.GetString("API_HASH") ??
        throw new Exception("You should provide finally an API_HASH env variable");

    var bot = new TelegramBotClient(bot_token, api_id, api_hash, dbConn);
    return bot;
}

var me = await bot.GetMe();
Console.WriteLine($"Bot Token Read\nInitiating @{me.Username}...");


//My dependencies - Handling Commands with Modules (Separated)
var commandHandler = new CommandHandler();
commandHandler.Add(new StartComm());
commandHandler.Add(new DownloadComm());
commandHandler.Add(new ButtonComm());

bot.OnMessage += AnyMessage;
bot.OnMessage += SlashCommand;
bot.OnUpdate += CallBackQuerys;

async Task CallBackQuerys(Update update)
{
    if (update.Type != UpdateType.CallbackQuery) return;
    Console.WriteLine(update);
}

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

#region Main
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
