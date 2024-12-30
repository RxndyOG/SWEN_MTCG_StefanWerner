
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Transactions;
using AuthenticationClasses;
using System;

namespace TCPserverClasses
{
    public class Routes
    {

        private TCPServer _server;
        private Authent _authent;

        private Dictionary<string, Dictionary<string, Delegate>> routes;

        private string _methodePost = "POST";
        private string _methodeGet = "GET";
        private string _methodePut = "PUT";

        public Routes(TCPServer server)
        {
            _authent = new Authent();
            _server = server; // Speichere die Instanz von TCPServer
        }

        public (string, string, List<Dictionary<string, object>>, string) CalcRoutes(NetworkStream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string request = reader.ReadLine();
            if (string.IsNullOrEmpty(request)) return ("ERR", "ERR", null, "ERR");

            string[] tokens = request.Split(' ');
            if (tokens.Length < 2) return ("ERR", "ERR", null, "ERR");

            string method = tokens[0];
            string url = tokens[1];

            var Content = new Body().getBodyHeader(request, reader);

            var deserialBody = new Body().getDeserialBody(Content.Item1);

            return (method, url, deserialBody, Content.Item2);
        }

        public void HandleRouting()
        {
            routes = new Dictionary<string, Dictionary<string, Delegate>>
            {
                { _methodeGet, new Dictionary<string, Delegate>() },
                { _methodePost, new Dictionary<string, Delegate>() },
                { _methodePut, new Dictionary<string, Delegate>() }
            };

            InitRoutes(routes);
        }

        public void InitRoutes(Dictionary<string, Dictionary<string, Delegate>> routes)
        {

            routes[_methodePost]["/users"] = new Func<List<Dictionary<string, object>>, NetworkStream, int>(_server.HandleUser);
            routes[_methodePost]["/sessions"] = new Func<List<Dictionary<string, object>>, NetworkStream, int>(_server.HandleSession);
            routes[_methodePost]["/packages"] = new Func<List<Dictionary<string, object>>, string, NetworkStream, int>(_server.HandlePackages);
            routes[_methodePost]["/transactions/packages"] = new Func<List<Dictionary<string, object>>, string, NetworkStream, int>(_server.HandleTransPackages);
            routes[_methodePost]["/battles"] = new Func<string, NetworkStream, int>(_server.HandleBattleList);
            routes[_methodePost]["/tradings"] = new Func<List<Dictionary<string, object>>, string, NetworkStream, int>(_server.HandleTradingPost);

            routes[_methodeGet]["/cards"] = new Func<string, NetworkStream, int>(_server.HandleStack);
            routes[_methodeGet]["/deck"] = new Func<string, NetworkStream, int>(_server.HandleDeck);
            routes[_methodeGet]["/usersControll"] = new Func<string, NetworkStream, int>(_server.HandleUserControllGET);
            routes[_methodeGet]["/stats"] = new Func<string, NetworkStream, int>(_server.HandleStats);
            routes[_methodeGet]["/scoreboard"] = new Func<NetworkStream, int>(_server.HandleLeaderboard);
            routes[_methodeGet]["/tradings"] = new Func<string, NetworkStream, int>(_server.HandleTrading);
            


            routes[_methodePut]["/deck"] = new Func<List<Dictionary<string, object>>, string, NetworkStream, int>(_server.HandleDeckInput);
            routes[_methodePut]["/usersControll"] = new Func<List<Dictionary<string, object>>, string, NetworkStream, int>(_server.HandleUserControllPUT);
        }

        public void HandleRequest(List<Dictionary<string, object>> deserialBody, string methode, string url, string Auth, NetworkStream stream)
        {

            if (_authent.AuthenticateRouteUsersControll(url, methode, Auth, deserialBody, stream, routes) == 0) { return; }

            int i = _authent.AuthenticateUnautherized(url, methode, Auth, routes);
            switch (i)
            {
                case -1:
                    Console.WriteLine("Unautherized - Url Error");
                    _server.SendResponse(stream, "404", "Unautherized");
                    return;
                    break;
                case -2:
                    Console.WriteLine("Unautherized - Methode Error");
                    _server.SendResponse(stream, "404", "Unautherized");
                    return;
                    break;
                default:
                    break;

            }

            int returnValue = _authent.AuthenticateRoutes(url, methode, Auth, deserialBody, stream, routes);

            switch (returnValue)
            {
                case 0:
                    return;
                    break;
                case -1:
                    _server.SendResponse(stream, "404", "Unautherized");    // route doesnt exist
                    break;
                case -2:
                    _server.SendResponse(stream, "404", "Unautherized");    // usercontroll Error exists
                    break;
                default:
                    Console.WriteLine("Error during Routing");
                    break;
            }
        }
    }
}
