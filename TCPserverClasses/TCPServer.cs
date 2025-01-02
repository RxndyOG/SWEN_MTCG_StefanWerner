using System.Net.Sockets;
using System.Net;
using UserClasses;
using packageClasses;
using BattleClasses;
using AuthenticationClasses;
using DatabaseClasses;
using TradingClasses;
using CardsClasses;

using System.Text;

namespace TCPserverClasses
{
    public class TCPServer
    {

        private int _port;
        private Routes _routes;

        private TcpListener _listener;
        private Authent _authent;
        private Battle _battle;
        private Database _database;

        private List<User> users;
        private List<Packages> packs;
        private TcpClient client;

        public TCPServer(int port)
        {
            _port = port;
            _routes = new Routes(this);
            users = new List<User>();
            _authent = new Authent();
            _battle = new Battle();
            _database = new Database();
            packs = new List<Packages>();

        }

        private static readonly object padlock = new object();

        private const string OK = "200";
        private const string Created = "201";
        private const string NotFound = "404";

        // startet den TCPListener
        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"Server running on port {_port}");
            lock (padlock)
            {
                users = _database.GetUser();
                users = _database.GetCards(users);
            }

            while (true)
            {
                client = _listener.AcceptTcpClient();
                Task.Run(() =>
                {
                    NetworkStream stream = client.GetStream();
                    var urlMethode = _routes.CalcRoutes(stream);

                    _routes.HandleRouting();
                    _routes.HandleRequest(urlMethode.Item3, urlMethode.Item1, urlMethode.Item2, urlMethode.Item4, stream);

                    client.Close();
                });
            }
        }
        
        // Handles the Battle system, this gets called when a client wants to Battle
        public int HandleBattleList(string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                if (UserAuthentic.Item1.SetGetCardsDeck.Count() < 4)
                {
                    Console.WriteLine("Not enough cards in deck");
                    SendResponse(stream, NotFound, "Error in Battle");
                    return -1;
                }

                var usersNew = _battle.CalculateBattleList(UserAuthentic.Item1, users);

                if (usersNew.Item1 != null)
                {
                    int index = users.FindIndex(s => s.SetGetUsername == usersNew.Item1.SetGetUsername);

                    if (index != -1)
                        users[index] = usersNew.Item1;
                    lock (padlock)
                    {
                        _database.updateUserValues(usersNew.Item1, index, usersNew.Item1.SetGetUsername);
                        _database.updateCardsValues(usersNew.Item1, index, usersNew.Item1.SetGetUsername);
                        _database.updateDecksValues(usersNew.Item1, index, usersNew.Item1.SetGetUsername);
                    }
                    index = users.FindIndex(s => s.SetGetUsername == usersNew.Item2.SetGetUsername);

                    if (index != -1)
                        users[index] = usersNew.Item2;

                    lock (padlock)
                    {
                        _database.updateUserValues(usersNew.Item2, index, usersNew.Item2.SetGetUsername);
                        _database.updateCardsValues(usersNew.Item1, index, usersNew.Item1.SetGetUsername);
                        _database.updateDecksValues(usersNew.Item1, index, usersNew.Item1.SetGetUsername);
                    }
                    SendResponse(stream, OK, "");
                    return 0;
                }
                SendResponse(stream, NotFound, "Error in Battle");
                return -1;
            }
            SendResponse(stream, NotFound, "Error in Battle");
            return -1;
        }

        public int HandleTradingTrade(List<Dictionary<string, object>> body, string Auth, string id, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                List<Trading> tradingDeals = new List<Trading>();
                lock (padlock)
                {
                    _database = new Database();
                    tradingDeals = _database.GetTradingDeals();
                }
                if (tradingDeals.Count() < 1)
                {
                    Console.WriteLine("No trades Open");
                    SendResponse(stream, OK, "Empty Trading List");
                    return -1;
                }
                foreach (Trading trading in tradingDeals)
                {

                    if (trading.SetGetUsername == UserAuthentic.Item1.SetGetUsername)
                    {
                        Console.WriteLine("You Can't trade with yourself");
                        SendResponse(stream, NotFound, "Trade Cant be completed");
                        return 1;
                    }
                    else
                    {
                        if (trading.SetGetTradingId == id)
                        {
                            List<User> tradePartner = _database.GetCards(users);
                            foreach (User user in tradePartner)
                            {
                                if (user.SetGetUsername == trading.SetGetUsername)
                                {
                                    foreach (Cards card in user.SetGetCardsStack)
                                    {
                                        if (card.SetGetID == trading.SetGetCardId)
                                        {
                                            lock (padlock)
                                            {
                                                if (card.SetGetDamage >= trading.SetGetDamage)
                                                {
                                                    
                                                    foreach (Cards acceptedCard in UserAuthentic.Item1.SetGetCardsStack)
                                                    {
                                                        foreach (var ElementBody in body)
                                                        {
                                                            if (ElementBody.ContainsKey("ID"))
                                                            {
                                                                if (acceptedCard.SetGetID == ElementBody["ID"].ToString())
                                                                {
                                                                    _database.InsertTradedCard(card, UserAuthentic.Item1.SetGetUsername);
                                                                    _database.DeleteTradedCard(card, user.SetGetUsername);

                                                                    _database.InsertTradedCard(acceptedCard, user.SetGetUsername);
                                                                    _database.DeleteTradedCard(acceptedCard, UserAuthentic.Item1.SetGetUsername);

                                                                    _database.updateUserValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, UserAuthentic.Item1.SetGetUsername);
                                                                    _database.updateCardsValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, UserAuthentic.Item1.SetGetUsername);
                                                                    _database.updateDecksValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, UserAuthentic.Item1.SetGetUsername);
                                                                    _database.updateUserValues(user, UserAuthentic.Item1.SetGetId + 1, user.SetGetUsername);
                                                                    _database.updateCardsValues(user, UserAuthentic.Item1.SetGetId + 1, user.SetGetUsername);
                                                                    _database.updateDecksValues(user, UserAuthentic.Item1.SetGetId + 1, user.SetGetUsername);

                                                                    _database.DeleteTradingDeal(user.SetGetUsername, trading.SetGetTradingId);

                                                                    Console.WriteLine("Trade Completed " + card.SetGetID + ": new Owner " + UserAuthentic.Item1.SetGetUsername);
                                                                    Console.WriteLine("Trade Completed " + acceptedCard.SetGetID + ": new Owner " + user.SetGetUsername);
                                                                    SendResponse(stream, OK, "Trade Completed");
                                                                    return 0;
                                                                }
                                                            }
                                                            
                                                        }
                                                        
                                                    }

                                                    SendResponse(stream, NotFound, "AcceptedCard Not in your Inventory");
                                                    Console.WriteLine("The Card Provided does not belong to you");
                                                    return 1;
                                                }

                                                SendResponse(stream, NotFound, "Not enough Damage for Min Damage Requirement");
                                                Console.WriteLine("The Damage Of the Card does not equal or excede the min damage");
                                                return 1;
                                            }
                                            

                                            return 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                SendResponse(stream, Created, "Trading List");
                return 0;
            }
            SendResponse(stream, NotFound, "User Doesn't Exist");
            return -1;
            return 1;
        }


        // POSTs the trading deal
        public int HandleTradingPost(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {

            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
          
                foreach (var elementBody in body)
                {
                    if (elementBody.ContainsKey("Id"))
                    {
                 
                        lock (padlock)
                        {
                            if (_database.testForIDWithGivenUsername(UserAuthentic.Item1.SetGetUsername, elementBody["CardToTrade"].ToString()) == true && _database.testIfTradingAlreadyExists(elementBody["CardToTrade"].ToString()) == false)
                            {
                                
                                Trading trading = new Trading();
                                trading.SetGetUsername = UserAuthentic.Item1.SetGetUsername;
                                trading.SetGetTradingId = elementBody["Id"].ToString();
                                trading.SetGetCardId = elementBody["CardToTrade"].ToString();
                                int cardType;
                                if (elementBody["Type"].ToString() == "monster")
                                {
                                    cardType = 0;
                                }
                                else
                                {
                                    cardType = 1;
                                }
                                trading.SetGetCardType = cardType;
                                trading.SetGetDamage = (float)Convert.ToDecimal(elementBody["MinimumDamage"]);
                                _database.InsertTradingDeal(trading);
                                Console.WriteLine("Trade Created");

                                SendResponse(stream, Created, "Trade Created");
                                return 0;
                            }
                            else
                            {
                                Console.WriteLine("Trade already exists");
                                SendResponse(stream, NotFound, "Trade Exists");
                                return 1;
                            }
                        }
                    }
                }
            }
            SendResponse(stream, NotFound, "User doesn't Exist");
            return -1;
        }

        // prints all the trading deals
        public int HandleTrading(string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                List<Trading> tradingDeals = new List<Trading>();
                lock (padlock)
                {
                    _database = new Database();
                    tradingDeals = _database.GetTradingDeals();
                }
                if (tradingDeals.Count() < 1)
                {
                    Console.WriteLine("No trades Open");
                    SendResponse(stream, OK, "Empty Trading List");
                    return -1;
                }
                foreach (Trading trading in tradingDeals)
                {
                    if (trading.SetGetUsername == UserAuthentic.Item1.SetGetUsername)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("TradingID: " + trading.SetGetTradingId);
                    Console.WriteLine("Username: " + trading.SetGetUsername);
                    Console.WriteLine("CardID: " + trading.SetGetCardId);
                    string cardType;
                    if (trading.SetGetCardType == 1)
                    {
                        cardType = "Spell";
                    }
                    else 
                    {
                        cardType = "Monster";
                    }
                    Console.WriteLine("Card Type: " + cardType);
                    Console.WriteLine("Min Damage: " + trading.SetGetDamage);
                    Console.WriteLine("--------------------------------");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                SendResponse(stream, Created, "Trading List");
                return 0;
            }
            SendResponse(stream, NotFound, "User Doesn't Exist");
            return -1;
        }

        // Deletes Trading deals
        public int HandleDeleteTrading(string Auth, string id, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                List<Trading> tradingDeals = new List<Trading>();
                lock (padlock)
                {
                    _database = new Database();
                    tradingDeals = _database.GetTradingDeals();
                }
                if (tradingDeals.Count() < 1)
                {
                    Console.WriteLine("No trades Open");
                    SendResponse(stream, OK, "Empty Trading List");
                    return -1;
                }
                foreach (Trading trading in tradingDeals)
                {
                   
                    if (trading.SetGetUsername == UserAuthentic.Item1.SetGetUsername)
                    {
                       
                        if (trading.SetGetTradingId == id)
                        {
                            _database.DeleteTradingDeal(trading.SetGetUsername, trading.SetGetTradingId);
                            Console.WriteLine("Trade Deleted");
                            SendResponse(stream, Created, "Trade Deleted");
                            return 0;
                        }
                    }
                }
                SendResponse(stream, Created, "Trading List");
                return 0;
            }
            SendResponse(stream, NotFound, "User Doesn't Exist");
            return -1;
            return 1;
        }

        // handles the leaderboard when the client wants to see it
        public int HandleLeaderboard(NetworkStream stream)
        {
            lock (padlock)
            {
                _database = new Database();
                users = _database.GetUser();
                users = _database.GetCards(users);
            }
            new Battle().printLeaderboard(users);
            SendResponse(stream, OK, "Leaderboard");
            return 0;
        }

        // handles the stats output ehen the client wants to see the stats
        public int HandleStats(string Auth, NetworkStream stream)
        {
            lock (padlock)
            {
                _database = new Database();
                users = _database.GetUser();
                users = _database.GetCards(users);
            }

            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                UserAuthentic.Item1.printStats();
                SendResponse(stream, OK, "user Stats");
                return 0;
            }
            SendResponse(stream, NotFound, "User doesn't Exist");
            return -1;
        }

        // handles the user Profil change when the user uses POST methode
        public int HandleUserControllPUT(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {

                foreach (var elementBody in body)
                {
                    if (elementBody.ContainsKey("Name"))
                    {
                        lock (padlock)
                        {
                            if (_database.testForUser(elementBody["Name"].ToString()) == true)
                            {
                                Console.WriteLine("User with given Username Already Exists!");
                                SendResponse(stream, NotFound, "User already Exists");
                                return -1;
                            }
                        }
                    }
                }

                string olduser = UserAuthentic.Item1.changeUserData(body);
                lock (padlock)
                {
                    _database.updateUserValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, olduser);
                    _database.updateCardsValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, olduser);
                    _database.updateDecksValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, olduser);

                }
                Console.WriteLine("User edited Profile");
                SendResponse(stream, OK, "New user Data");
                return 0;
            }
            SendResponse(stream, NotFound, "User doesn't Exist");
            return -1;
        }

        // handles the user Profile change on GET methode
        public int HandleUserControllGET(string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                UserAuthentic.Item1.printUserData();
                SendResponse(stream, OK, "User Printed");
                return 0;
            }
            SendResponse(stream, NotFound, "User doesnt Exist");
            return -1;
        }

        // Hanbles the deck creation when the user wants to put cards in deck
        public int HandleDeckInput(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                int i = UserAuthentic.Item1.addToDeck(body);
                switch (i)
                {
                    case -2:
                        SendResponse(stream, NotFound, "Bad Request");
                        return -1;
                        break;
                    case -1:
                        SendResponse(stream, NotFound, "Not enough ids");
                        return -1;
                        break;
                    case 0:
                        lock (padlock)
                        {
                            _database.saveDeck(UserAuthentic.Item1, _database.testForDeck(UserAuthentic.Item1));
                        }
                        SendResponse(stream, OK, "Cards added to Deck");
                        return 0;
                        break;
                    default:
                        return -1;
                        break;
                }
            }
            return 0;
        }

        // handles the deck print as a json object
        public int HandleDeckOther(string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                UserAuthentic.Item1.printStackDeckOther(1);
                SendResponse(stream, OK, "list of all cards in Deck");
                return 0;
            }
            return -1;
        }

        // handles the deck print out
        public int HandleDeck(string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                UserAuthentic.Item1.printStackDeck(1);
                SendResponse(stream, OK, "list of all cards in Deck");
                return 0;
            }
            return -1;
        }

        // handles the stack print out
        public int HandleStack(string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {
                UserAuthentic.Item1.printStackDeck(0);
                SendResponse(stream, OK, "list of all cards");
                return 0;
            }
            return 1;
        }

        // handles the transfer of cards to the user
        public int HandleTransPackages(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {

                int prevCount = UserAuthentic.Item1.SetGetCardsStack.Count();

                var packsINT = UserAuthentic.Item1.openPackage(packs);
                packs = packsINT.Item1 as List<Packages>;
                lock (padlock)
                {
                    _database.updateUserValues(UserAuthentic.Item1, (UserAuthentic.Item1.SetGetId + 1), UserAuthentic.Item1.SetGetUsername);
                    _database.updateCardsValues(UserAuthentic.Item1, (UserAuthentic.Item1.SetGetId + 1), UserAuthentic.Item1.SetGetUsername);
                    _database.updateDecksValues(UserAuthentic.Item1, (UserAuthentic.Item1.SetGetId + 1), UserAuthentic.Item1.SetGetUsername);
                    _database.InsertUserCards(UserAuthentic.Item1, prevCount);
                }
                switch (packsINT.Item2)
                {
                    case 0:
                        SendResponse(stream, Created, "Card added");
                        break;
                    case -1:
                        SendResponse(stream, NotFound, "Not enough Money");
                        break;
                    case -2:
                        SendResponse(stream, NotFound, "Not enough Packages");
                        break;
                    default:
                        break;
                }
            }
            return 0;
        }

        // handles the creation of packages by the admin
        public int HandlePackages(List<Dictionary<string, object>> receive, string Auth, NetworkStream stream)
        {

            var packagesINT = new Packages().createPackage(Auth, receive);

            if (packagesINT.Item2 == 0)
            {
                packs.Add(packagesINT.Item1);
                Console.WriteLine("Packages created by Admin");
                SendResponse(stream, Created, "");
                return 0;
            }
            SendResponse(stream, NotFound, "not Autherized");
            return -1;
        }

        //erstellt neuen User
        public int HandleUser(List<Dictionary<string, object>> receive, NetworkStream stream)
        {
            string testToken = receive[0]["Username"].ToString() + "-mtcgToken";
            if (users.FirstOrDefault(j => j != null && j.SetGetToken == testToken) != null)
            {
                Console.WriteLine("User Already Exists");
                SendResponse(stream, NotFound, "User already exists");
                return 1;

            }

            if (users.FirstOrDefault(j => j != null && j.SetGetUsername == receive[0]["Username"].ToString()) == null)      //testet ob die erste stelle der liste der username ist und schaut ob er in der liste users exisitert
            {

                users.Add(new User().createUser(Authent.HashPassword(receive[0]["Password"].ToString()), receive[0]["Username"].ToString(), users.Count()));
                lock (padlock)
                {
                    _database.insertIntoDatabase(users[users.Count() - 1].SetGetUsername, users[users.Count() - 1].SetGetPassword);
                }
                SendResponse(stream, Created, "");
                return 0;
            }

            Console.WriteLine("User Already Exists");
            SendResponse(stream, NotFound, "User already exists");
            return 1;

        }

        // handles the user login
        public int HandleSession(List<Dictionary<string, object>> receive, NetworkStream stream)
        {
            User userExists = users.FirstOrDefault(j => j != null && j.SetGetUsername == receive[0]["Username"].ToString());

            if (userExists != null)
            {

                if (userExists.SetGetToken != null && userExists.SetGetToken.Contains("-mtcgToken"))
                {
                    Console.WriteLine("User token already exists");
                    SendResponse(stream, NotFound, "token already exists");
                    return 1;
                }

                string token = userExists.loginUser(receive[0]["Username"].ToString(), receive[0]["Password"].ToString());

                lock (padlock)
                {
                    if (_database.testForToken(token) == true)
                    {
                        Console.WriteLine("User token already exists");
                        Console.WriteLine("Token will be recalculated!");
                        DateTime time = DateTime.Now;
                        token = token + time.ToString();
                        Console.WriteLine("Current token: " + token);
                    }

                    _database.updateUserValues(userExists, userExists.SetGetId + 1, userExists.SetGetUsername);
                    _database.updateCardsValues(userExists, userExists.SetGetId + 1, userExists.SetGetUsername);
                    _database.updateDecksValues(userExists, userExists.SetGetId + 1, userExists.SetGetUsername);
                }


                if (token == "ERR")
                {
                    Console.WriteLine("login failed");
                    SendResponse(stream, NotFound, "Login failed");
                    return 1;
                }

                Console.WriteLine("User Token created: " + token);
                SendResponse(stream, OK, token);
                return 0;
            }
            SendResponse(stream, NotFound, "No User Exists");
            return 1;
        }

        // sends a response to the user
        public void SendResponse(NetworkStream stream, string status, string body)
        {
            string response = $"HTTP/1.1 {status}\r\n" +
                              "Content-Type: text/plain\r\n" +
                              $"Content-Length: {body.Length}\r\n" +
                              "\r\n" +
                              body;
            byte[] buffer = Encoding.UTF8.GetBytes(response);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
