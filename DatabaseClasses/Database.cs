using CardsClasses;
using Npgsql;
using System.Security;
using System.Xml.Linq;
using UserClasses;
using TradingClasses;
using System.Net.Sockets;

namespace DatabaseClasses
{

    public class Database
    {

        public Database()
        {
            string connectionString = $"Host={host};Username={username};Password={password};Database={database};Port=5432";
            connection = new NpgsqlConnection(connectionString);
        }

        private string host = "localhost";
        private string username = "RxndyOG";
        private string password = "technikum";
        private string database = "swendatabase";

        private NpgsqlConnection connection;

        private readonly object connectionLock = new object();

        // connects to the database
        private void OpenConnection()
        {
            lock (connectionLock)
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
            }
        }

        // disconnects from the database
        private void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        // tests if user exists
        public bool testForUser(string username)
        {

            OpenConnection();
            string query = "SELECT COUNT(*) FROM users WHERE username = @username";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("username", username);
                var count = (long)cmd.ExecuteScalar();

                if (count > 0)
                {

                    CloseConnection();
                    return true;
                }
                CloseConnection();
                return false;

            }
        }

        // tests if token exists
        public bool testForToken(string token)
        {

            OpenConnection();
            string query = "SELECT COUNT(*) FROM users WHERE token = @token";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("token", token);
                var count = (long)cmd.ExecuteScalar();

                if (count > 0)
                {

                    CloseConnection();
                    return true;
                }
                CloseConnection();
                return false;

            }
        }

        public bool testIfTradingAlreadyExists(string id)
        {
            OpenConnection();
            string query = "SELECT COUNT(*) FROM trading WHERE cardid = @cardid";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("cardid", id);
                var count = (long)cmd.ExecuteScalar();

                if (count > 0)
                {

                    CloseConnection();
                    return true;
                }
                CloseConnection();
                return false;

            }
        }

        public bool testForIDWithGivenUsername(string username, string ID)
        {
            OpenConnection();
            string query = "SELECT card_id FROM cards WHERE \"user\" = @user";
        

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("user", username);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        string testedID = string.Empty;
                        testedID = !reader.IsDBNull(0) ? reader.GetString(0) : string.Empty;
                     

                        if (testedID == ID)
                        {
                            CloseConnection();
                            return true;
                        }
                    }
                }

            }
            CloseConnection();
            return false;
        }

        // gets all the cards from deck from user
        public List<User> GetCards(List<User> users)
        {
            for (int i = 0; i < users.Count(); i++)
            {
                users[i].SetGetCardsStack = GetUserCards(users[i].SetGetUsername);
                users[i].SetGetCardsDeck = getSavedDeck(users[i]);
                if (users[i].SetGetCardsDeck.Count() > 0)
                {


                    List<Cards> cardsTempDeck = users[i].SetGetCardsDeck;

                    users[i].AddSavedCardsToDeck(cardsTempDeck);
                }

            }

            return users;
        }

        // get all the users 
        public List<User> GetUser()
        {
            OpenConnection();

            List<User> users = new List<User>();

            string query = "SELECT * FROM users";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User();

                        user.SetGetId = Convert.ToInt32(reader["id"]);
                        user.SetGetUsername = reader["username"] != DBNull.Value ? reader["username"].ToString() : string.Empty;
                        user.SetGetPassword = reader["password"] != DBNull.Value ? reader["password"].ToString() : string.Empty;
                        user.SetGetToken = reader["token"] != DBNull.Value ? reader["token"].ToString() : string.Empty;
                        user.SetGetBio = reader["bio"] != DBNull.Value ? reader["bio"].ToString() : string.Empty;
                        user.SetGetCoins = reader["coins"] != DBNull.Value ? Convert.ToInt32(reader["coins"]) : 0;
                        user.SetGetWins = reader["wins"] != DBNull.Value ? Convert.ToInt32(reader["wins"]) : 0;
                        user.SetGetLose = reader["lose"] != DBNull.Value ? Convert.ToInt32(reader["lose"]) : 0;

                        users.Add(user);
                    }
                }
            }

            CloseConnection();
            return users;
        }

        // tests if user exists
        private bool UserExists(string username)
        {
            OpenConnection();

            string query = "SELECT COUNT(*) FROM users WHERE Username = @username";
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                CloseConnection();
                return count > 0;
            }
        }

        // get the decks saved in database
        private List<Cards> getSavedDeck(User user)
        {
            List<Cards> userCards = new List<Cards>();

            OpenConnection();

            string query = "SELECT cardid1, cardid2, cardid3, cardid4 FROM deck WHERE \"username\" = @username;";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("username", user.SetGetUsername);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) 
                    {
                       
                        for (int i = 0; i < 4; i++) 
                        {
                            Cards card = new Cards();
                            card.SetGetID = !reader.IsDBNull(i) ? reader.GetString(i) : string.Empty;
                            userCards.Add(card);
                        }
                    }
                }
            }

            CloseConnection();
            return userCards;
        }

        // test if deck exists
        public bool testForDeck(User user)
        {
            OpenConnection();

            string query = "SELECT COUNT(*) FROM deck WHERE \"username\" = @username";

            using (var cmd = new NpgsqlCommand(query, connection))
            {

                cmd.Parameters.AddWithValue("username", user.SetGetUsername);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                CloseConnection();
                return count > 0;
            }
        }

        // saves the deck of the user
        public void saveDeck(User user, bool deckExists)
        {
            if (deckExists)
            {
                OpenConnection();

                string query = "UPDATE deck SET cardid1 = @cardid1, cardid2 = @cardid2, cardid3 = @cardid3, cardid4 = @cardid4 WHERE \"username\" = @username;";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd.Parameters.AddWithValue("cardid1", user.SetGetCardsDeck[0].SetGetID);
                    cmd.Parameters.AddWithValue("cardid2", user.SetGetCardsDeck[1].SetGetID);
                    cmd.Parameters.AddWithValue("cardid3", user.SetGetCardsDeck[2].SetGetID);
                    cmd.Parameters.AddWithValue("cardid4", user.SetGetCardsDeck[3].SetGetID);
                    int rowsAffected = cmd.ExecuteNonQuery();

                }

                CloseConnection();
                return;
            }
            else
            {
                OpenConnection();

                string query = "INSERT INTO deck (username, cardid1, cardid2, cardid3, cardid4) VALUES (@username, @cardid1, @cardid2, @cardid3, @cardid4);";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd.Parameters.AddWithValue("cardid1", user.SetGetCardsDeck[0].SetGetID);
                    cmd.Parameters.AddWithValue("cardid2", user.SetGetCardsDeck[1].SetGetID);
                    cmd.Parameters.AddWithValue("cardid3", user.SetGetCardsDeck[2].SetGetID);
                    cmd.Parameters.AddWithValue("cardid4", user.SetGetCardsDeck[3].SetGetID);
                    int rowsAffected = cmd.ExecuteNonQuery();

                }

                CloseConnection();
                return;
            }
        }

        // saves the users pwd and username
        public void insertIntoDatabase(string user, string pwd)
        {

            if (UserExists(user))
            {
                Console.WriteLine("Benutzer existiert bereits.");
                return;
            }


            OpenConnection();

            string query = "INSERT INTO users (Username, Password) VALUES (@user, @pwd);";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("user", user);
                cmd.Parameters.AddWithValue("pwd", pwd);

                int rowsAffected = cmd.ExecuteNonQuery();

            }

            Console.WriteLine("User Created");

            CloseConnection();
        }

        // get all cards from user
        private List<Cards> GetUserCards(string username)
        {
            List<Cards> userCards = new List<Cards>();

            OpenConnection();

            string query = "SELECT card_id, card_name, damage, family, cardtype, element FROM cards WHERE \"user\" = @username;";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        Cards card = new Cards();
                        card.SetGetID = !reader.IsDBNull(0) ? reader.GetString(0) : string.Empty;
                        card.SetGetName = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                        card.SetGetDamage = !reader.IsDBNull(2) ? (float)Convert.ToDecimal(reader["damage"]) : 0;
                        card.SetGetFamily = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                        card.SetGetCardType = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0;
                        card.SetGetElement = !reader.IsDBNull(5) ? reader.GetString(5) : string.Empty;
                        userCards.Add(card);
                    }
                }
            }

            CloseConnection();
            return userCards;
        }

        // saves user cards
        public void InsertUserCards(User user, int count)
        {
            OpenConnection();

            List<string> cardIDtest = new List<string>();

            string cardIDquery = "SELECT card_id FROM cards WHERE \"user\" = @username;";

            using (var cmd = new NpgsqlCommand(cardIDquery, connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cardId = reader.ToString();
                        cardIDtest.Add(cardId);
                    }
                }
            }


            for (int i = count; i < user.SetGetCardsStack.Count(); i++)
            {
                string query = "Insert INTO cards (card_id, card_name, damage, family, cardtype,element, \"user\") VALUES (@card_id,@card_name,@damage,@family,@cardtype,@element,@user);";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    Cards card = user.SetGetCardsStack[i];

                    string test = cardIDtest.FirstOrDefault(j => j != null && j == card.SetGetID);

                    if (test != null)
                    {
                        Console.WriteLine("card in Deck");
                        continue;
                    }

                    cmd.Parameters.AddWithValue("card_id", card.SetGetID);
                    cmd.Parameters.AddWithValue("card_name", card.SetGetName);
                    cmd.Parameters.AddWithValue("damage", card.SetGetDamage);
                    cmd.Parameters.AddWithValue("family", card.SetGetFamily ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("cardtype", card.SetGetCardType);
                    cmd.Parameters.AddWithValue("element", card.SetGetElement ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("user", user.SetGetUsername ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                }
            }
            CloseConnection();
        }

        public List<Trading> GetTradingDeals()
        {
            List<Trading> tradingDeals = new List<Trading>();

            OpenConnection();

            string query = "SELECT id, \"username\", cardid, type, damage FROM trading;";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        Trading card = new Trading();
                        card.SetGetTradingId = !reader.IsDBNull(0) ? reader.GetString(0) : string.Empty; ;
                        card.SetGetUsername = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                        card.SetGetCardId = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                        card.SetGetCardType = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0;
                        card.SetGetDamage = !reader.IsDBNull(4) ? (float)Convert.ToDecimal(reader["damage"]) : 0;

                        tradingDeals.Add(card);
                    }
                }
            }

            CloseConnection();
            return tradingDeals;
        } 

        public void DeleteTradingDeal(string username, string id)
        {
            OpenConnection();

            string query = "DELETE FROM trading WHERE id = @id AND \"username\" = @username";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@username", username);

                int rowsAffected = cmd.ExecuteNonQuery();
   
            }

            CloseConnection();
            return;
        }

        public void InsertTradedCard(Cards card, string newUser)
        {
            OpenConnection();

            string query = "INSERT INTO cards (card_id, \"user\", card_name, damage, family, cardtype, element) values (@card_id, @username, @card_name, @damage, @family, @cardtype, @element)";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("card_id", card.SetGetID);
                cmd.Parameters.AddWithValue("username", newUser);
                cmd.Parameters.AddWithValue("card_name", card.SetGetName);
              
                cmd.Parameters.AddWithValue("damage", card.SetGetDamage);
                cmd.Parameters.AddWithValue("family", card.SetGetFamily);
                cmd.Parameters.AddWithValue("cardtype", card.SetGetCardType);
                cmd.Parameters.AddWithValue("element", card.SetGetElement);

                int rowsAffected = cmd.ExecuteNonQuery();

            }

            CloseConnection();
        }

        public void DeleteTradedCard(Cards card, string olduser)
        {
            OpenConnection();

            string query = "DELETE FROM cards WHERE card_id = @card_id AND \"user\" = @username";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("card_id", card.SetGetID);
                cmd.Parameters.AddWithValue("username", olduser);

                int rowsAffected = cmd.ExecuteNonQuery();
            }

            CloseConnection();
            return;
        }

        public void InsertTradingDeal(Trading trading)
        {
            OpenConnection();

            string query = "Insert INTO trading (id, \"username\", cardid, type, damage) values (@id, @username, @cardid, @type, @damage)";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("id", trading.SetGetTradingId);
                cmd.Parameters.AddWithValue("cardid", trading.SetGetCardId);
                cmd.Parameters.AddWithValue("username", trading.SetGetUsername);
                cmd.Parameters.AddWithValue("damage", trading.SetGetDamage);
                cmd.Parameters.AddWithValue("type", trading.SetGetCardType);
                cmd.Parameters.AddWithValue("damage", trading.SetGetDamage);

                int rowsAffected = cmd.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public void updateDecksValues(User user, int id, string olduser)
        {
            OpenConnection();

            try
            {
                using (var cmd3 = new NpgsqlCommand("UPDATE deck SET username = @username WHERE username = @olduser", connection))
                {
                    cmd3.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd3.Parameters.AddWithValue("olduser", olduser);

                    cmd3.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        public void updateCardsValues(User user, int id, string olduser)
        {
           
            OpenConnection();

            try
            {
                using (var cmd2 = new NpgsqlCommand("UPDATE cards SET \"user\" = @username WHERE \"user\" = @olduser", connection))
                {
                    cmd2.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd2.Parameters.AddWithValue("olduser", olduser);

                    cmd2.ExecuteNonQuery();
                }

            }
            finally
            {
                CloseConnection();
            }
        }

        public void updateUserValues(User user, int id, string olduser)
        {
            OpenConnection();

            try
            {
                using (var cmd1 = new NpgsqlCommand("UPDATE users SET username = @username, password = @password, token = @token, coins = @coins, image = @image, bio = @bio, wins = @wins, lose = @lose WHERE username = @olduser", connection))
                {
                    cmd1.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd1.Parameters.AddWithValue("password", user.SetGetPassword);
                    cmd1.Parameters.AddWithValue("token", user.SetGetToken);
                    cmd1.Parameters.AddWithValue("coins", user.SetGetCoins);
                    cmd1.Parameters.AddWithValue("image", user.SetGetImage);
                    cmd1.Parameters.AddWithValue("bio", user.SetGetBio);
                    cmd1.Parameters.AddWithValue("wins", user.SetGetWins);
                    cmd1.Parameters.AddWithValue("lose", user.SetGetLose);
                    cmd1.Parameters.AddWithValue("olduser", olduser);

                    cmd1.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }


    }
}
