using System.Net.Sockets;
using System.Net;
using UserClasses;
using packageClasses;
using BattleClasses;
using AuthenticationClasses;
using DatabaseClassses;

using System.Text;
using System.Runtime.Serialization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

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

        private const string OK = "200";
        private const string Created = "201";
        private const string NotFound = "404";

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"Server running on port {_port}");

            users = _database.GetUser();
            users = _database.GetCards(users);

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Task.Run(() =>
                {
                    NetworkStream stream = client.GetStream();
                    var urlMethode = _routes.CalcRoutes(stream);

                    _routes.HandleRouting();
                    _routes.HandleRequest(urlMethode.Item3, urlMethode.Item1, urlMethode.Item2, urlMethode.Item4, stream);

                    client.Close();
                });
            }
             /*
            while (true)
            {

                TcpClient client = _listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                var urlMethode = _routes.CalcRoutes(stream);

                _routes.HandleRouting();

                _routes.HandleRequest(urlMethode.Item3, urlMethode.Item1, urlMethode.Item2, urlMethode.Item4, stream);

                client.Close();
            }
             */
        }

        public int HandleBattleList(string Auth, NetworkStream stream)
        {

            Console.WriteLine("Battle wird für jetzt geskipped. Es funktioniert alles in der Theory die Logik geht nur die Warteliste geht nicht. alles andere was attacken angeht geht.");
            Console.WriteLine("Daher wird die warteliste momentan geskipped und der Battle startet automatisch ohne der Warteliste");

            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {

                var usersNew = _battle.CalculateBattleList(UserAuthentic.Item1, users);

                if (usersNew.Item1 != null)
                {
                    int index = users.FindIndex(s => s.SetGetUsername == usersNew.Item1.SetGetUsername);

                    if (index != -1)
                        users[index] = usersNew.Item1;

                    _database.updateUserValues(usersNew.Item1, index, usersNew.Item1.SetGetUsername);
                    index = users.FindIndex(s => s.SetGetUsername == usersNew.Item2.SetGetUsername);

                    if (index != -1)
                        users[index] = usersNew.Item2;

                    _database.updateUserValues(usersNew.Item2, index, usersNew.Item2.SetGetUsername);
                }
                SendResponse(stream, OK, "");
                return 0;
            }
            SendResponse(stream, NotFound, "Error in Battle");
            return -1;
        }

        public int HandleTradingPost(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {
            _database = new Database();
            users = _database.GetUser();
            users = _database.GetCards(users);
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

        public int HandleTrading(string Auth, NetworkStream stream)
        {
            _database = new Database();
            users = _database.GetUser();
            users = _database.GetCards(users);
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

        public int HandleLeaderboard(NetworkStream stream)
        {
            _database = new Database();
            users = _database.GetUser();
            users = _database.GetCards(users);
            new Battle().printLeaderboard(users);
            SendResponse(stream, OK, "Leaderboard");
            return 0;
        }

        public int HandleStats(string Auth, NetworkStream stream)
        {
            _database = new Database();
            users = _database.GetUser();
            users = _database.GetCards(users);
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

        public int HandleUserControllPUT(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {

                foreach (var elementBody in body)
                {
                    if (elementBody.ContainsKey("Name"))
                    {
                        if (_database.testForUser(elementBody["Name"].ToString()) == true)
                        {
                            Console.WriteLine("User with given Username Already Exists!");
                            SendResponse(stream, NotFound, "User already Exists");
                            return -1;
                        }
                    }
                }

                string olduser = UserAuthentic.Item1.changeUserData(body);
                _database.updateUserValues(UserAuthentic.Item1, UserAuthentic.Item1.SetGetId + 1, olduser);

                Console.WriteLine("User edited Profile");
                SendResponse(stream, OK, "New user Data");
                return 0;
            }
            SendResponse(stream, NotFound, "User doesn't Exist");
            return -1;
        }

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
                        _database.saveDeck(UserAuthentic.Item1);
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

        public int HandleTransPackages(List<Dictionary<string, object>> body, string Auth, NetworkStream stream)
        {
            var UserAuthentic = _authent.IsAuthentic(Auth, users);
            if (UserAuthentic.Item2 == 0)
            {

                int prevCount = UserAuthentic.Item1.SetGetCardsStack.Count();

                var packsINT = UserAuthentic.Item1.openPackage(packs);
                packs = packsINT.Item1 as List<Packages>;

                _database.updateUserValues(UserAuthentic.Item1, (UserAuthentic.Item1.SetGetId + 1), UserAuthentic.Item1.SetGetUsername);

                _database.InsertUserCards(UserAuthentic.Item1, prevCount);

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

            if (users.FirstOrDefault(j => j != null && j.SetGetUsername == receive[0]["Username"].ToString()) == null)      //testet ob die erste stelle der liste der username ist und schaut ob er in der liste users exisitert
            {
                users.Add(new User().createUser(Authent.HashPassword(receive[0]["Password"].ToString()), receive[0]["Username"].ToString(), users.Count()));
                _database.insertIntoDatabase(users[users.Count()-1].SetGetUsername, users[users.Count()-1].SetGetPassword);
                SendResponse(stream, Created, "");
                return 0;
            }

            Console.WriteLine("User Already Exists");
            SendResponse(stream, NotFound, "User already exists");
            return 1;
        }

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

                if (_database.testForToken(token) == true)
                {
                    Console.WriteLine("User token already exists");
                    Console.WriteLine("Token will be recalculated!");
                    DateTime time = DateTime.Now;
                    token = token + time.ToString();
                    Console.WriteLine("Current token: " + token);
                }

                _database.updateUserValues(userExists, userExists.SetGetId + 1, userExists.SetGetUsername);

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
