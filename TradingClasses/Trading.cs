using System.Security;

namespace TradingClasses
{
    public class Trading
    {
        public Trading() { }

        private string tradingID;
        private string username;
        private string cardid;
        private int type;
        private float damage;

        public string SetGetTradingId
        {
            get => tradingID;
            set => tradingID = value;
        }

        public string SetGetUsername
        {
            get => username;
            set => username = value;
        }

        public string SetGetCardId
        {
            get => cardid;
            set => cardid = value;
        }

        public int SetGetCardType
        {
            get => type;
            set => type = value;
        }

        public float SetGetDamage
        {
            get => damage;
            set => damage = value;
        }

    }
}
