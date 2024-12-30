using System.Globalization;

namespace CardsClasses
{
    public class Cards
    {
        public Cards()
        {
            CardFamily = new List<string>
            {
                "Goblin",
                "Dragon",
                "Wizzard",
                "Ork",
                "Knight",
                "Kraken",
                "Elve"
            };
            CardElement = new List<string>
            {
                "Fire",
                "Water"
            };
        }

        private string ID;
        private string Name;
        private float Damage;
        private string Family;
        private int CardType;
        private string Element;

        public int winCount = 0;

        List<string> CardFamily;
        List<string> CardElement;

        public string SetGetElement
        {
            get => Element; set => Element = value;
        }

        public int SetGetCardType
        {
            get => CardType;
            set => CardType = value;
        }

        public string SetGetFamily
        {
            get => Family;
            set => Family = value;
        }

        public string SetGetID
        {
            get => ID;
            set => ID = value;
        }

        public string SetGetName
        {
            get => Name;
            set => Name = value;
        }

        public float SetGetDamage
        {
            get => Damage;
            set => Damage = value;
        }

        public Cards createCard(string id, string name, float damage)
        {
            Cards card = new Cards();

            card.SetGetID = id;
            card.SetGetName = name;
            card.SetGetDamage = damage;

            card.exportCardDetails();

            return card;
        }

        private void parseCardType(string name)
        {
            if (name.Contains("Spell"))
            {
                CardType = 1;
            }
            else
            {
                CardType = 0;
            }
        }

        private void parseName(string name)
        {
            for(int i = 0; i < CardElement.Count(); i++)
            {
                if (name.Contains(CardElement[i]))
                {
                    Element = CardElement[i];
                    break;
                }
                if (i == CardElement.Count()-1)
                {
                    Element = "Normal";
                    break;
                }
            }

            for (int i = 0; i < CardFamily.Count(); i++)
            {
                if (name.Contains(CardFamily[i]))
                {
                    Family = CardFamily[i];
                    break;
                }
            }
            return;
        }

        public virtual void exportCardDetails()
        {
            parseCardType(SetGetName);
            parseName(SetGetName);

            return;
        }

    }
}
