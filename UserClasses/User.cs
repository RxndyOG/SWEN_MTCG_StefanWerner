using packageClasses;
using CardsClasses;
using System.Reflection;
using System.Security.Cryptography;

namespace UserClasses
{

    public class User
    {

        private string _tokenString = "-mtcgToken";

        public User()
        {
            stack = new List<Cards>();
            deck = new List<Cards>();
            Wins = 0;
            Lose = 0;
        }

        private int ID;
        private List<Cards> stack;
        private List<Cards> deck;
        private string pwd;
        private string username;
        private string token;
        private int coins = 16;

        private string Bio = string.Empty;
        private string Image = string.Empty;
                                  
        private int Wins;
        private int Lose;
        private int Elo;

        public int SetGetId
        {
            get => ID; set => ID = value;
        }

        public string SetGetImage
        {
            get => Image; set => Image = value;
        }

        public int SetGetCoins
        {
           get => coins; set => coins = value;
        }

        public string SetGetBio
        {
            get => Bio; set => Bio = value;
        }

        public int SetGetLose
        {
            get => Lose; set => Lose = value;
        }

        public int SetGetWins
        {
            get => Wins; set => Wins = value;
        }

        public string SetGetUsername
        {
            get => username; set => username = value;
        }

        public string SetGetPassword
        {
            get => pwd; set => pwd = value;
        }

        public string SetGetToken
        {
            get => token; set => token = value;
        }

        public int SetGetElo
        {
            get => 100 + (Wins * 3 - Lose * 5);
        }

        public List<Cards> SetGetCardsStack
        {
            get => stack;
            set => stack = value;
        }
        public List<Cards> SetGetCardsDeck
        {
            get => deck;
            set => deck = value;
        }

        public void printStats()
        {
            Console.WriteLine("------------------");
            Console.WriteLine("Wins: " + Wins);
            Console.WriteLine("Lose: " + Lose);
            Console.WriteLine("Elo: " + SetGetElo);
            Console.WriteLine("------------------");
            return;
        }

        public string changeUserData(List<Dictionary<string, object>> body)
        {

            string oldusername = SetGetUsername;
            string oldpwd = SetGetPassword;
            string oldImage = SetGetImage;
            string oldBio = SetGetBio;

            foreach (var elementBody in body)
            {
                if (elementBody.ContainsKey("Name"))
                {
                   
                    SetGetUsername = elementBody["Name"].ToString();
                }

                if (elementBody.ContainsKey("Password"))
                {
                    pwd = elementBody["Password"].ToString();
                }

                if (elementBody.ContainsKey("Image"))
                {
                    Image = elementBody["Image"].ToString();
                }

                if (elementBody.ContainsKey("Bio"))
                {
                    Bio = elementBody["Bio"].ToString();
                }
            }

            return oldusername;
        }

        public void printUserData()
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Username: " + SetGetUsername);
            Console.WriteLine("Password: " + pwd);
            Console.WriteLine("Bio: " + Bio);
            Console.WriteLine("Image: " + Image);
            Console.WriteLine("Token: " + SetGetToken); 
            Console.WriteLine("Coins: " + coins);
            Console.WriteLine("--------------------------------");
            return;
        }

        public void AddSavedCardsToDeck(List<Cards> TempDeck)
        {
            List<Cards> TempTempDeck = new List<Cards>(TempDeck);
       
            deck.Clear();
          
            foreach (var card in TempTempDeck)
            {
                
                Cards cardExists = stack.FirstOrDefault(j => j != null && j.SetGetID == card.SetGetID);
                if (cardExists != null)
                {
                    deck.Add(cardExists);
                }
                else
                {
                    return;
                }
            }
        }

        public int addToDeck(List<Dictionary<string, object>> body)
        {
            if(body.Count() != 4)
            {
                Console.WriteLine("Not enough Ids");
                return -1;
            }

            if (deck.Count == 4)
            {
                foreach (var id in body) 
                { 
                    Cards cardExists = stack.FirstOrDefault(j => j != null && j.SetGetID == id["Value"].ToString());
                    if (cardExists == null) 
                    {
                        Console.WriteLine("Card not in your Deck");
                        return -2;
                    }
                }
                
                deck.Clear();
            }

            foreach (var id in body)
            {
                Cards cardExists = stack.FirstOrDefault(j => j != null && j.SetGetID == id["Value"].ToString());
                if (cardExists != null)
                {
                    deck.Add(cardExists);
                    Console.WriteLine("Card added to deck");
                }
                else
                {
                    Console.WriteLine("Card not in your Deck");
                    return -2;
                }
            }
            return 0;
        }

        public void printStackDeck(int i)
        {
            if (i == 1)
            {
                if (deck.Count == 0)
                {
                    Console.WriteLine("No cards in Deck");
                    return;
                }
                if (deck.Count < 4)
                {
                    Console.WriteLine("Not enough Cards in Deck");
                    return;
                }
                foreach (var card in deck)
                {
                    if (card.SetGetCardType == 0) { Console.ForegroundColor = ConsoleColor.Red; }
                    if (card.SetGetCardType == 1) { Console.ForegroundColor = ConsoleColor.Green; }
                    Console.WriteLine(card.SetGetID);
                    Console.WriteLine(card.SetGetName);
                    Console.WriteLine(card.SetGetDamage + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return;
            }
            else
            {
                if (stack.Count == 0)
                {
                    Console.WriteLine("No cards in Stack");
                    return;
                }
                foreach (var card in stack)
                {
                    if (card.SetGetCardType == 0){ Console.ForegroundColor = ConsoleColor.Red; }
                    if (card.SetGetCardType == 1) { Console.ForegroundColor = ConsoleColor.Green; }
                    Console.WriteLine(card.SetGetID);
                    Console.WriteLine(card.SetGetName);
                    Console.WriteLine(card.SetGetDamage + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return;
            }
        }

        public (List<Packages>, int) openPackage(List<Packages> packs)
        {
            if (packs.Count != 0)
            {
                if (coins >= 4)
                {
                    foreach (var card in packs[0].getPack)
                    {
                        card.exportCardDetails();
                        stack.Add(card);
                    }
                    coins -= 4;
                    packs.RemoveAt(0);
                    Console.WriteLine("Cards added to: " + SetGetUsername);
                    return (packs, 0);
                }
                Console.WriteLine("Not Enough Coins!");
                return (packs, -1);
            }
            Console.WriteLine("Not Enough Packages");
            return (packs, -2);
        }

        public User createUser(string pwd, string username, int ID)
        {

            User user = new User();
            user.pwd = pwd;
            user.username = username;
            user.SetGetId = ID;

            return user;
        }

        public virtual bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        public string loginUser(string user, string password)
        {
            if (username == user && VerifyPassword(password, pwd))
            {
                token = username + "-mtcgToken";
                return token;
            }

            return "ERR";

        }

    }
}
