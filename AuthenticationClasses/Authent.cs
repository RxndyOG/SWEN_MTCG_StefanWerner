using System.IO;
using System.Net.Sockets;
using System;
using UserClasses;
using System.Reflection;
using System.Security.Cryptography;
using System.Net;


namespace AuthenticationClasses
{
    public class Authent
    {

        public Authent()
        {

        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public (User, int) IsAuthentic(string Auth, List<User> users)
        {
            User userExists = users.FirstOrDefault(j => j != null && j.SetGetToken == Auth.Replace("Bearer ", "").Trim());
            if (userExists != null)
            {
                return (userExists, 0);

            }
            return (null,-1);
        }

        public int AuthenticateRouteUsersControll(string url, string methode, string Auth, List<Dictionary<string, object>> deserialBody, NetworkStream stream, Dictionary<string, Dictionary<string, Delegate>> routes)
        {
            if (url.Contains("/users/"))
            {
                if (url.Contains(Auth.Replace("Bearer ", "").Trim().Replace("-mtcgToken", "").Trim()))
                {
                    if (methode == "GET")
                    {
                        var searchedRouteShort = routes[methode]["/usersControll"] as Func<string, NetworkStream, int>;
                        searchedRouteShort.Invoke(Auth, stream);
                        return 0;
                    }
                    var searchedRoute = routes[methode]["/usersControll"] as Func<List<Dictionary<string, object>>, string, NetworkStream, int>;
                    searchedRoute.Invoke(deserialBody, Auth, stream);
                    return 0;
                }
            }
            return 1;
        }

        public int AuthenticateUnautherized(string url, string methode, string Auth, Dictionary<string, Dictionary<string, Delegate>> routes)
        {
            if (!routes.ContainsKey(methode))
            {
                return -2;
            }
            else
            {
                if (!routes[methode].ContainsKey(url))
                {
                    return -1;
                }
            }
            return 0;
        }

        private static int GetDelegateParameterCount(Delegate del)
        {
            Type delegateType = del.GetType();

            MethodInfo methodInfo = delegateType.GetMethod("Invoke");

            return methodInfo.GetParameters().Length;
        }

        public int AuthenticateRoutesNonAuthNonArgs(string url, string methode, string Auth, List<Dictionary<string, object>> args, NetworkStream stream, Dictionary<string, Dictionary<string, Delegate>> routes)
        {
            if (Auth == null)
            {
                if (args == null)
                {
                    Console.WriteLine("Unautherized");
                    return -2;
                }
                var searchedRoute = routes[methode][url] as Func<List<Dictionary<string, object>>, NetworkStream, int>;
                searchedRoute.Invoke(args, stream);
                return 0;
            }
            return 1;
        }

        public int AuthenticateRoutesAuthNoArgs(string url, string methode, string Auth, List<Dictionary<string, object>> args, NetworkStream stream, Dictionary<string, Dictionary<string, Delegate>> routes)
        {
            if (Auth != null)
            {

                Delegate routeDelegate = routes[methode][url];
                int parameterCount = GetDelegateParameterCount(routeDelegate);
                if (parameterCount == 0)
                {                                                                                
                    var searchedRouteShort = routes[methode][url] as Func<int>;
                    searchedRouteShort.Invoke();
                    return 0;
                }
                if ((args == null) && parameterCount == 1)
                {
                    var searchedRouteShort = routes[methode][url] as Func<NetworkStream, int>;
                    searchedRouteShort.Invoke(stream);
                    return 0;
                }
                if ((args == null) && parameterCount == 2)
                {
                    var searchedRouteShort = routes[methode][url] as Func<string, NetworkStream, int>;
                    searchedRouteShort.Invoke(Auth, stream);
                    return 0;
                }
                var searchedRoute = routes[methode][url] as Func<List<Dictionary<string, object>>, string, NetworkStream, int>;
                searchedRoute.Invoke(args, Auth, stream);
                return 0;
            }
            return -1;
        }

        public int AuthenticateRoutes(string url, string methode, string Auth, List<Dictionary<string, object>> args, NetworkStream stream, Dictionary<string, Dictionary<string, Delegate>> routes)
        {

            int i = AuthenticateRoutesNonAuthNonArgs(url ,methode, Auth, args, stream, routes);

            if(i == -2 || i == 0) { return i; }  

            i = AuthenticateRoutesAuthNoArgs(url, methode, Auth, args, stream, routes);

            if(i == 0) { return i; }
            
            return -1;
        }
    }
}
