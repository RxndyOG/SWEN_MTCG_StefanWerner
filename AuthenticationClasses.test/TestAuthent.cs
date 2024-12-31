
using NUnit.Framework;
using NSubstitute;
using System.Net.Sockets;
using UserClasses;
using TCPserverClasses;
using System.Reflection;

namespace AuthenticationClasses.test
{
    [TestFixture]
    public class TestAuthent
    {
        private Dictionary<string, Dictionary<string, Delegate>> _mockRoutes;
        private Authent _classUnderTest;

        [SetUp]
        public void Setup()
        {
            
            _classUnderTest = new Authent();
            
            _mockRoutes = new Dictionary<string, Dictionary<string, Delegate>>();
        }

        // Testet, ob 0 zurückgegeben wird, wenn  Methode und URL in den Routen enthalten sind
        // should Succede
        [Test]
        public void testAuthenticateUnautherized_scenarioWithCorrectInputShouldReturn0()
        {
            // ARRANGE
            string url = "/users";
            string methode = "GET";
            string auth = "Bearer valid-token";

            _mockRoutes["GET"] = new Dictionary<string, Delegate>
            {
                { "/users", new Func<string, NetworkStream, int>((authToken, stream) => 0) }
            };

            // Act
            var result = _classUnderTest.AuthenticateUnautherized(url, methode, auth, _mockRoutes);

            // ASSERT
            Assert.AreEqual(0, result);
        }

        // Testet, ob -2 zurückgegeben wird, wenn Methode falsch ist
        // should Succede
        [Test]
        public void testAuthenticateUnautherized_scenarioWithWrongMethode()
        {
            // ARRANGE
            string url = "/users";
            string methode = "POST";
            string auth = "Bearer valid-token";

            // Act
            var result = _classUnderTest.AuthenticateUnautherized(url, methode, auth, _mockRoutes);

            // ASSERT
            Assert.AreEqual(-2, result);
        }

        // Testet, ob -1 zurückgegeben wird, wenn URL falsch ist
        // should Succede
        [Test]
        public void testAuthenticateUnautherized_scenarioWithWrongURL()
        {
            // ARRANGE
            string url = "/unknownUrl"; 
            string methode = "GET";
            string auth = "Bearer valid-token";

            _mockRoutes["GET"] = new Dictionary<string, Delegate>
            {
                { "/users", new Func<string, NetworkStream, int>((authToken, stream) => 0) }
            };

            // Act
            var result = _classUnderTest.AuthenticateUnautherized(url, methode, auth, _mockRoutes);

            // ASSERT
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        // testet ob der User Authentic ist
        // should succede
        [Test]
        public void testIsAuthentic_scenarioWithCorrectInput()
        {
            // ARRANGE
            var authToken = "Bearer abc123-mctgToken";
            var authenticator = new Authent();
            User user = new User();
            List<User> _users = new List<User>() { 
                new User { SetGetUsername = "user1", SetGetToken = "abc123-mctgToken" },
                new User { SetGetUsername = "user2", SetGetToken = "def456-mctgToken" } 
            };

            // ACT
            var result = authenticator.IsAuthentic(authToken, _users);

            // ASSERT
            Assert.AreEqual(0, result.Item2);
            Assert.AreEqual("user1", result.Item1.SetGetUsername);
        }

        // testet ob der User Authentic ist
        // should not be the same
        [Test]
        public void testIsAuthentic_scenarioWithIncorrectInput()
        {
            // ARRANGE
            var authToken = "Bearer ghi123-mctgToken";
            var authenticator = new Authent();
            User user = new User();
            List<User> _users = new List<User>() {
                new User { SetGetUsername = "user1", SetGetToken = "abc123-mctgToken" },
                new User { SetGetUsername = "user2", SetGetToken = "def456-mctgToken" }
            };

            // ACT
            var result = authenticator.IsAuthentic(authToken, _users);

            // ASSERT
            Assert.AreNotEqual(0, result.Item2);
            Assert.AreEqual(null, result.Item1);
        }

        // testet ob zwei parameter
        // should succede
        [Test]
        public void testGetDelegateParameterCount_scenarioWith2Parameters()
        {
            // ARRANGE
            Func<int, string, bool> func = (x, y) => true;

            // ACT
            var result = InvokePrivateMethod(func);

            // ASSERT
            Assert.AreEqual(2, result); 
        }

        // testet ob ein parameter existiert
        // should succede
        [Test]
        public void testGetDelegateParameterCount_scenarioWithOneParameter()
        {
            // ARRANGE
            Action<int> action = (x) => { };

            // ACT 
            var result = InvokePrivateMethod(action);

            // ASSERT
            Assert.AreEqual(1, result); 
        }

        // testet ohne parameter
        // should succede
        [Test]
        public void testGetDelegateParameterCount_scenarioWithNoParamters()
        {
            // ARRANGE
            Func<bool> func = () => true;

            // ACT
            var result = InvokePrivateMethod(func);

            // ASSERT
            Assert.AreEqual(0, result); 
        }

    
        private static int GetDelegateParameterCount(Delegate del)
        {
            Type delegateType = del.GetType();
            MethodInfo methodInfo = delegateType.GetMethod("Invoke");
            return methodInfo.GetParameters().Length;
        }

        private static int InvokePrivateMethod(Delegate del)
        {
            MethodInfo methodInfo = typeof(TestAuthent)
                                    .GetMethod("GetDelegateParameterCount", BindingFlags.NonPublic | BindingFlags.Static);
            return (int)methodInfo.Invoke(null, new object[] { del });
        }

    }
}