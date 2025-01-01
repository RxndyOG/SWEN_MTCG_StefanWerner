using CardsClasses;
using System.Globalization;

namespace packageClasses
{
    public class Packages
    {
        public Packages()
        {
            cardsInPack = new List<Cards>();
        }

        List<Cards> cardsInPack;

        // creates packages with the given values by admin
        public (Packages, int) createPackage(string Auth, List<Dictionary<string, object>> body)
        {
            string inputToken = Auth.Replace("Bearer ", "").Trim();

            if (inputToken == "admin-mtcgToken")
            {
                if (body.Count() != 5)
                {
                    Console.WriteLine("Not enough Cards");
                    return (null, -1);
                }

                Packages package = new Packages();
                foreach (var bodyElement in body)
                {
                    Cards card = new Cards();
                    card = card.createCard(bodyElement["Id"].ToString(), bodyElement["Name"].ToString(), (float.Parse(bodyElement["Damage"].ToString(), CultureInfo.InvariantCulture)));
                    package.cardsInPack.Add(card);
                }
                return (package, 0);
            }
            return (null, -2);
        }

        public virtual List<Cards> getPack 
        {
            get => cardsInPack;
        }
    }
}
