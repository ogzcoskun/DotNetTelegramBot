using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTelegramBot.Models.CoinListModels
{
    public class CoinModel
    {
        [JsonProperty("IsSuccess")]
        public bool IsSuccess;

        [JsonProperty("ErrorCode")]
        public string ErrorCode;

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage;

        [JsonProperty("StatusCode")]
        public int StatusCode;

        [JsonProperty("Data")]
        public List<Datum> Data;
    }
}
