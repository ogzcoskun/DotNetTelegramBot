using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTelegramBot.Models.CoinListModels
{
    public class Datum
    {
        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("Code")]
        public string Code;

        [JsonProperty("OriginalCode")]
        public string OriginalCode;

        [JsonProperty("DepositMaxLimit")]
        public decimal DepositMaxLimit;

        [JsonProperty("DepositMinLimit")]
        public decimal DepositMinLimit;

        [JsonProperty("WithdrawMaxLimit")]
        public decimal WithdrawMaxLimit;

        [JsonProperty("WithdrawMinLimit")]
        public decimal WithdrawMinLimit;

        [JsonProperty("WithdrawFee")]
        public decimal WithdrawFee;

        [JsonProperty("Confirmations")]
        public decimal Confirmations;
    }
}
