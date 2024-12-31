using CardsClasses;
using NSubstitute;
using packageClasses;
using System.IO;
using System.Security.Principal;

namespace UserClasses.Test
{
    public class TestUser
    {
        //These Tests Where the way i learned how to use Unit Testing so thats why some Tests might be weird

        [SetUp]
        public void Setup()
        {
        }

        // testet ob die Stats Richtig Ausgegeben werden und ob die Elo richtig berechnet werden
        // should SUCCED
        [Test]
        public void testPrintStats_scenarioWithResult()
        {
            //ARRANGE
            User testedUser = new User();
            testedUser.SetGetWins = 10;
            testedUser.SetGetLose = 5;

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            //ACT
            testedUser.printStats();

            //ASSERT
            var output = stringWriter.ToString().Trim();
            Assert.AreEqual("------------------\r\nWins: 10\r\nLose: 5\r\nElo: 105\r\n------------------", output);
        }

        // testet ob die Stats Richtig Ausgegeben werden und ob die Elo richtig berechnet werden
        // should not be the same
        [Test]
        public void testPrintStats_scenarioShouldFail()
        {
            //ARRANGE
            User testedUser = new User();
            testedUser.SetGetWins = 10;
            testedUser.SetGetLose = 5;

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            //ACT
            testedUser.printStats();

            //ASSERT
            var output = stringWriter.ToString().Trim();
            Assert.AreNotEqual("------------------\r\nWins: 10\r\nLose: 5\r\nElo: 110\r\n------------------", output);
        }

        //testet ob user mit leerem Username erstellt werden Können
        // should SUCCED
        [Test]
        public void testCreateUser_scenarioWithEmptyFields()
        {
            // ARRANGE

            string expectedPwd = "";
            string expectedUsername = "";
            int expectedId = 0;

            // ACT
            User user = new User().createUser(expectedPwd, expectedUsername, expectedId);

            // ASSERT
            Assert.AreEqual(expectedPwd, user.SetGetPassword);
            Assert.AreEqual(expectedUsername, user.SetGetUsername);
            Assert.AreEqual(expectedId, user.SetGetId);
        }

        //testet ob user mit leerem Username erstellt werden Können
        // should SUCCED
        [Test]
        public void testCreateUser_scenarioWithNonEmptyFields()
        {
            // ARRANGE

            string expectedPwd = "Max";
            string expectedUsername = "maxi1234";
            int expectedId = 2;

            // ACT
            User user = new User().createUser(expectedPwd, expectedUsername, expectedId);

            // ASSERT
            Assert.AreEqual(expectedPwd, user.SetGetPassword);
            Assert.AreEqual(expectedUsername, user.SetGetUsername);
            Assert.AreEqual(expectedId, user.SetGetId);
        }

        // user login testen mit correcten input und mit MOCKING
        // should SUCCED
        [Test]
        public void testLoginUser_scenarioWithCorrectValues()
        {
            // ARRANGE
            var user = Substitute.For<User>();
            user.SetGetUsername = "max";
            user.SetGetPassword = "hashedpassword";

            user.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            // ACT
            var token = user.loginUser("max", "password");

            // ASSERT
            Assert.AreEqual("max-mtcgToken", token);
        }

        // user login testen mit falschen input und mit MOCKING
        // should not be the same
        [Test]
        public void testLoginUser_scenarioWithIncorrectValues()
        {
            // ARRANGE
            var user = Substitute.For<User>();
            user.SetGetUsername = "maximus";
            user.SetGetPassword = "hashedpasswordIncorrect";

            user.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            // ACT
            var token = user.loginUser("testuser", "password"); 

            // ASSERT
            Assert.AreNotEqual("max-mtcgToken", token);
        }

        // Open Package mit correct values MOCKING (sollte o returnen für keine fehler, 1 für anzahl an coins left und 4 für die anzahl der cards die der user hat)
        // should SUCCEDE
        [Test]
        public void testOpenPackage_scenarioWithEnoughCoinsAndPackages_ShouldAddCardsToStack()
        {
            // ARRANGE
            var packagesMock = Substitute.For<Packages>();
            var cardMock1 = Substitute.For<Cards>();
            var cardMock2 = Substitute.For<Cards>();

            packagesMock.getPack.Returns(new List<Cards> { cardMock1, cardMock2, cardMock1, cardMock2 });

            var packs = new List<Packages> { packagesMock };

            var user = new User();
            user.SetGetCoins = 5; 
            user.SetGetCardsStack = new List<Cards>();

            // ACT
            var result = user.openPackage(packs);

            // ASSERT
            Assert.AreEqual(0, result.Item2); 
            Assert.AreEqual(1, user.SetGetCoins);
            Assert.AreEqual(4, user.SetGetCardsStack.Count);
        }

        // testet ob der user karten aus dem stack dem deck hinzufügen kann
        // should Succede
        [Test]
        public void testAddToDeck_scenarioWithCorrectInput()
        {
            // ARRANGE
            var user = new User();

            var cardMock1 = Substitute.For<Cards>();
            cardMock1.SetGetID.Returns("123");

            var cardMock2 = Substitute.For<Cards>();
            cardMock2.SetGetID.Returns("456");

            var cardMock3 = Substitute.For<Cards>();
            cardMock3.SetGetID.Returns("789");

            var cardMock4 = Substitute.For<Cards>();
            cardMock4.SetGetID.Returns("101");

            user.SetGetCardsStack = new List<Cards> { cardMock1, cardMock2, cardMock3, cardMock4 };

          
            var body = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { "Value", "123" } },
            new Dictionary<string, object> { { "Value", "456" } },
            new Dictionary<string, object> { { "Value", "789" } },
            new Dictionary<string, object> { { "Value", "101" } },
        };

            // ACT
            var result = user.addToDeck(body);

            // ASSERT
            Assert.AreEqual(0, result);
            Assert.AreEqual(4, user.SetGetCardsDeck.Count); 
        }

        // testet ob der user karten aus dem stack dem deck hinzufügen kann
        // should not be the same
        // should return -2 to indicate that id in stack is not same as choosen id from user
        [Test]
        public void testAddToDeck_scenarioWithIncorrectInput()
        {
            // ARRANGE
            var user = new User();

            var cardMock1 = Substitute.For<Cards>();
            cardMock1.SetGetID.Returns("123");

            var cardMock2 = Substitute.For<Cards>();
            cardMock2.SetGetID.Returns("456");

            var cardMock3 = Substitute.For<Cards>();
            cardMock3.SetGetID.Returns("789");

            var cardMock4 = Substitute.For<Cards>();
            cardMock4.SetGetID.Returns("102");

            user.SetGetCardsStack = new List<Cards> { cardMock1, cardMock2, cardMock3, cardMock4 };


            var body = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { "Value", "123" } },
            new Dictionary<string, object> { { "Value", "456" } },
            new Dictionary<string, object> { { "Value", "789" } },
            new Dictionary<string, object> { { "Value", "101" } },
        };

            // ACT
            var result = user.addToDeck(body);

            // ASSERT
            Assert.AreNotEqual(0, result);
            Assert.AreNotEqual(4, user.SetGetCardsDeck.Count);
        }

    }
}