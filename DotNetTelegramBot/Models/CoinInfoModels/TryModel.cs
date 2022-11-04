using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTelegramBot.Models.CoinInfoModels
{
    public class TryModel
    {
        public double Price { get; set; }
        public double ChangeDay { get; set; }
        public double ChangePctDay { get; set; }
        public double HighDay { get; set; }
        public double LowDay { get; set; }
        public double HighDay24H { get; set; }
        public double LowDay24H { get; set; }
        public double AverageSale { get; set; }
        public double VolumeDay { get; set; }
    }
}
