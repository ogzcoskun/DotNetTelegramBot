using System;
using DotNetTelegramBot.Models.CoinInfoModels;
using DotNetTelegramBot.Models.CoinListModels;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using Microsoft.Extensions.Hosting;
using System.Linq;
using DotNetTelegramBot.LoggingFile;
using System.Security.Policy;
using NLog;
using Telegram.Bot.Types.InputFiles;
using System.Net;
using System.IO;
using System.Data;
using System.Drawing;
using ScottPlot;
using System.Runtime.Intrinsics.X86;

namespace DotNetTelegramBot
{
    public class BotService : BackgroundService, IAsyncDisposable
    {
        private static ILoggerManager _logger;

        public BotService(ILoggerManager looger)
        {
            _logger = looger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Run();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            };
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogManager.Shutdown();
            await base.StartAsync(cancellationToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            await base.StartAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            LogManager.Shutdown();
            await Task.CompletedTask;
        }

        public void Run()
        {
            try
            {

                Console.WriteLine("Please Enter Your TelegramBot Key Then Press Enter.");
                var connectionString = Console.ReadLine();

                TelegramBotClient Bot = new TelegramBotClient(connectionString);

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new UpdateType[]
                    {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                    }
                };

                Bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);
                Console.WriteLine("Bot is active!\nSend a text to bot that says \"Help\" in Telegram to see the commands list!");



                Console.WriteLine(".\n.\n.\n.\n.\n.\nType \"Stop\" then hit enter to stop program.");


                while (true)
                {
                    var stopKey = Console.ReadLine();

                    if (stopKey == "Stop")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("You need to type \"Stop\" here in order to stop program running!");
                    }
                }
            }
            catch(Exception ex)
            {
                //_logger.WithProperty("url", "")
                //    .WithProperty("shortdesc", "")
                //    .WithProperty("requestdata", "")
                //    .LogInfo($"TELEGRAMBOT --- {ex.Message}");
                Run();
            }

        }

        private static Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return Task.CompletedTask;
        }

        private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {

            if (update.Type == UpdateType.Message && update.Message.Text != null && update.Message.Type.ToString() == "Text")
            {
                var message = update.Message.Text.Replace("i", "I").ToUpper();

                if (update.Message.Text == null)
                {
                    await bot.SendTextMessageAsync(update.Message.Chat.Id, $"The data type is not correct, please send text only!");
                }
                else if (update.Message.Text.ToLower().Contains("price/"))
                {
                    message = Regex.Replace(message, @"\s+", "");
                    await SendPrice(message, bot, update);
                }
                else if (update.Message.Text.Contains("Info/"))
                {
                    message = Regex.Replace(message, @"\s+", "");
                    await SendInfo(message, bot, update);
                }
                else if (update.Message.Text.Contains("Help"))
                {
                    message = Regex.Replace(message, @"\s+", "");
                    var commandsInfo = await GetCommandList(message);
                    await bot.SendTextMessageAsync(update.Message.Chat.Id, commandsInfo);
                }else if (update.Message.Text.ToUpper().Contains("CHART/"))
                {
                    message = Regex.Replace(message, @"\s+", "");
                    await SendChart(message, bot, update);
                }
                else
                {
                    await bot.SendTextMessageAsync(update.Message.Chat.Id, $"Module doesn't exist!!!   Type \"Help\" to see the command list  {update.Message.Chat.Id}.");
                }
            }
        }

        private static async Task SendPrice(string message, ITelegramBotClient bot, Update update)
        {

            var coinList = await GetCoinList();
            var assetsString = message.Split("/")[1];
            var assetList = assetsString.Split(",");

            var messageToSend = "💲 PRICES ARE: \n \n";

            foreach (var asset in assetList)
            {
                if (coinList.Contains(asset.ToUpper()))
                {
                    var price = await GetAllCoinInfo(asset.ToUpper());

                    var lira = price.TRY == null ? 0 : price.TRY.Price;
                    var dolar = price.USDT == null ? 0 : price.USDT.Price;

                    messageToSend += $"{asset.ToUpper()}   →     TRY: {String.Format("{0:n}", Math.Round(lira, 2))}   ◻   USDT: {String.Format("{0:n}", Math.Round(dolar, 2))} \n \n";
                }
                else
                {
                    messageToSend += "Given asset doesn't exist in Coin List \n \n";
                }
            }
            await bot.SendTextMessageAsync(update.Message.Chat.Id, messageToSend);
        }

        private static async Task SendInfo(string message, ITelegramBotClient bot, Update update)
        {

            var coinList = await GetCoinList();
            var asset = message.Split("/")[1];
            var messageToSend = "💲 Coin Info: \n \n";

            if (coinList.Contains(asset.ToUpper()))
            {

                //var increaseString = "⬆";
                //var decreaseString = "⬇️";

                var info = await GetAllCoinInfo(asset.ToUpper());
                messageToSend += $"   Asset: {asset}\n\n\n◽   Currency: TRY \n◾   Price: {String.Format("{0:n}", Math.Round(info.TRY.Price, 2))} \n◾   ChangeDay: {String.Format("{0:n}", Math.Round(info.TRY.ChangeDay, 2))} {await Get24HourStatus(info.TRY.ChangeDay)} \n◾   ChangePctDay: {String.Format("{0:n}", Math.Round(info.TRY.ChangePctDay, 2))}\n◾   HighDay: {String.Format("{0:n}", Math.Round(info.TRY.HighDay, 2))} \n◾   LowDay: {String.Format("{0:n}", Math.Round(info.TRY.LowDay, 2))} \n◾   HighDay24H: {String.Format("{0:n}", Math.Round(info.TRY.HighDay24H, 2))} \n◾   LowDay24H: {String.Format("{0:n}", Math.Round(info.TRY.LowDay24H, 2))} \n◾   VolumeDay: {String.Format("{0:n}", Math.Round(info.TRY.VolumeDay, 2))}\n\n◽   Currency: USDT \n◾   Price: {String.Format("{0:n}", Math.Round(info.USDT.Price, 2))} \n◾   ChangeDay: {String.Format("{0:n}", Math.Round(info.USDT.ChangeDay, 2))} {await Get24HourStatus(info.USDT.ChangeDay)} \n◾   ChangePctDay: {String.Format("{0:n}", Math.Round(info.USDT.ChangePctDay, 2))}\n◾   HighDay: {String.Format("{0:n}", Math.Round(info.USDT.HighDay, 2))} \n◾   LowDay: {String.Format("{0:n}", Math.Round(info.USDT.LowDay, 2))} \n◾   HighDay24H: {String.Format("{0:n}", Math.Round(info.USDT.HighDay24H, 2))} \n◾   LowDay24H: {String.Format("{0:n}", Math.Round(info.USDT.LowDay24H, 2))} \n◾   VolumeDay: {String.Format("{0:n}", Math.Round(info.USDT.VolumeDay, 2))}";
            }
            else
            {
                messageToSend += "Given asset doesn't exist in Coin List \n \n";
            }

            await bot.SendTextMessageAsync(update.Message.Chat.Id, messageToSend);
        }

        private static async Task SendChart(string message, ITelegramBotClient bot, Update update)
        {
            try
            {
                message.ToUpper();

                var assetsString = message.Split("/")[1];
                var assetList = assetsString.Split(",");

                foreach (var asset in assetList)
                {
                    asset.ToUpper();
                }

                if (assetList.Length != 2)
                {
                    await bot.SendTextMessageAsync(update.Message.Chat.Id, "Given Assets are wrong");
                }
                else
                {
                    var hourStr = message.ToUpper();
                    var hour = (hourStr.Contains("24H")) ? 24 : 1;

                    var guid = await GetMarketGuid(assetList[0], assetList[1]);

                    if (guid == null)
                    {
                        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Given Assets are wrong");
                    }
                    else
                    {
                        var client = new RestClient($"http://157.230.21.25:8000/chart-image");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        request.AddParameter("fsym", assetList[0]);
                        request.AddParameter("tsym", assetList[1]);
                        request.AddParameter("mguid", guid);
                        request.AddParameter("interval", hour);
                        var response = await client.ExecuteAsync(request);

                        var responseDS = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

                        var url = responseDS["chartUrl"];

                        WebClient myWebClient = new WebClient();

                        Stream myStream = myWebClient.OpenRead(url);
                        StreamReader sr = new StreamReader(myStream);

                        var imageToSend = new InputOnlineFile(myStream);

                        await bot.SendPhotoAsync(update.Message.Chat.Id, imageToSend);
                    }
                }
            }
            catch (Exception ex)
            {
                await bot.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
            }      
        }

        private static async Task<List<string>> GetCoinList()
        {
            try
            {
                var client = new RestClient($"https://apiv2.coinpara.com/api/coin/list");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                var response = await client.ExecuteAsync(request);

                var listToDeserialize = response.Content;

                var listToDS = JsonConvert.DeserializeObject<CoinModel>(listToDeserialize);

                var coinList = new List<string>();

                foreach (var coin in listToDS.Data)
                {
                    coinList.Add(coin.Code.ToString());
                }
                return coinList;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<CoinInfoModel> GetAllCoinInfo(string asset)
        {

            var client = new RestClient($"https://apiv2.coinpara.com/api/coin/price");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddParameter("assets", asset);
            var response = await client.ExecuteAsync(request);

            var ds = JsonConvert.DeserializeObject<Dictionary<string, CoinInfoModel>>(response.Content);

            var coinInfo = ds[asset.ToUpper()];

            return coinInfo;
        }

        private static async Task<string> GetCommandList(string message)
        {
            var messageEn = "Türkçe için HelpTR yazmanız yeterlidir.\n\nCommands List:\r\n\r\nPrice: Provides the instant USDT and TL values ​​of the coin list provided by the user.\r\n\n◽Exp:       Price/BTC,ETH,XRP\r\n\r\nInfo: Provides information about the coin's changes in the last 24 hours\r\n\n◽Exp:       Info/BTC\r\n\r\nChart: Provides the chart of the given assets. If you put \"24H\" infront of the command it will provide 24 hour chart. If you dont place anything before \"Chart\" command, it will provide 1 hour Chart.\r\n\n◽Exp:       Chart/BTC,TRY\n\n◽Exp:       24HChart/BTC,TRY\n\n You can try the examples above. Simply just copy and paste the examples.";

            var messageTr = "Komut Listesi:\r\n\r\nPrice: Kullanıcı tarafından sağlanan Coin listesinin anlık USDT ve TL değerlerini sağlar..\r\n\n◽Örn:       Price/BTC,ETH,XRP\r\n\r\nInfo: Coin'in son 24 saatteki değişiklikleri hakkında bilgi sağlar\r\n\n◽Örn:       Info/BTC\r\n\r\nChart: Verilen varlıkların grafiğini sağlar. Komutun önüne \"24H\" koyarsanız, 24 saatlik grafik sağlayacaktır. \"Chart\" komutundan önce herhangi bir şey koymazsanız, 1 saatlik Grafik sağlayacaktır.\r\n\n◽Örn:       Chart/BTC,TRY\n\n◽Örn:       24HChart/BTC,TRY\n\n Yukarıdaki örnekleri deneyebilirsiniz. Örnekleri kopyalayıp yapıştırmanız yeterlidir.";

            message.ToUpper();
            if (message.Contains("TR"))
            {
                return messageTr;
            }
            else
            {
                return messageEn;
            }

            //return "Commands List:\r\n\r\nPrice: Provides the instant USDT and TL values ​​of the coin list provided by the user.\r\n\n◽Exp:       Price/BTC,ETH,XRP\r\n\r\nInfo: Provides information about the coin's changes in the last 24 hours\r\n\n◽Exp:       Info/BTC\r\n\r\nChart: Provides the chart of the given assets. If you put \"24H\" infront of the command it will provide 24 hour chart. If you dont place anything before \"Chart\" command, it will provide 1 hour Chart.\r\n\n◽Exp:       Chart/BTC,TRY\n\n◽Exp:       24HChart/BTC,TRY\n\n You can try the examples above. Simply just copy and paste the examples.";
        }

        private static async Task<string> Get24HourStatus(double value)
        {
            if (value >= 0)
            {
                return "🟢";
            }
            else
            {
                return "🔴";
            }
        }

        private static async Task<string> GetMarketGuid(string asset1, string asset2)
        {

            try
            {
                asset1 = asset1.Replace("i", "I");
                asset2 = asset2.Replace("i", "I");

                var client = new RestClient($"https://apiv2.coinpara.com/api/coins/getAll/true");
                client.Timeout = -1;

                var request = new RestRequest(Method.GET);
                var response = await client.ExecuteAsync(request);

                var data = ((JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content))["Data"]).ToString();
                var dataIn = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);

                var guid = String.Empty;

                foreach (var quote in dataIn)
                {
                    if (quote["Code"].ToString() == asset1)
                    {
                        var idList = JsonConvert.DeserializeObject<Dictionary<string, string>>(quote["MarketGuids"].ToString());

                        foreach (var id in idList)
                        {
                            if (id.Key == asset2)
                            {
                                guid = id.Value;
                                break;
                            }
                        }
                    }
                }

                if (guid == String.Empty)
                {
                    return null;
                }
                else
                {
                    return guid;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

    }
}

