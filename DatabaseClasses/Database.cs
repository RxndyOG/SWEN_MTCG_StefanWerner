using CardsClasses;
using Npgsql;
using System.Security;
using System.Xml.Linq;
using UserClasses;

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

        private void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

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

        /*
        public User getUserExact(int userID)
        {
            OpenConnection();

            string query = "SELECT * FROM users WHERE id = @userId";

            User user = new User();

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("userId", userID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        user.SetGetId = userID;
                        user.SetGetUsername = reader["username"] != DBNull.Value ? reader["username"].ToString() : string.Empty;
                        user.SetGetPassword = reader["password"] != DBNull.Value ? reader["password"].ToString() : string.Empty;
                        user.SetGetToken = reader["token"] != DBNull.Value ? reader["token"].ToString() : string.Empty;
                        user.SetGetBio = reader["bio"] != DBNull.Value ? reader["bio"].ToString() : string.Empty;
                        user.SetGetCoins = reader["coins"] != DBNull.Value ? Convert.ToInt32(reader["coins"]) : 0;
                        user.SetGetWins = reader["wins"] != DBNull.Value ? Convert.ToInt32(reader["wins"]) : 0;
                        user.SetGetLose = reader["lose"] != DBNull.Value ? Convert.ToInt32(reader["lose"]) : 0;
                    }
                }
            }

            CloseConnection();
            return user;
        }
        */

        public List<User> GetUser()
        {
            OpenConnection();

            List<User> users = new List<User>();

            string query = "SELECT * FROM users"; // Abfrage alle Benutzer

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) // Alle Benutzer durchlaufen
                    {
                        User user = new User();

                        user.SetGetId = Convert.ToInt32(reader["id"]); // Benutzer-ID setzen
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

        private bool UserExists(string username)
        {
            OpenConnection();

            string query = "SELECT COUNT(*) FROM users WHERE Username = @username";
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                CloseConnection();
                return count > 0; // Gibt true zurück, wenn der Benutzer existiert
            }
        }

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
                    if (reader.Read()) // Prüft, ob eine Zeile verfügbar ist
                    {
                        // Liest die Kartenwerte aus der Zeile
                        for (int i = 0; i < 4; i++) // Annahme: Es gibt 4 Karten-IDs
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

        public bool testForDeck(User user)
        {
            OpenConnection();

            string query = "SELECT COUNT(*) FROM deck WHERE \"username\" = @username";

            using (var cmd = new NpgsqlCommand(query, connection))
            {

                cmd.Parameters.AddWithValue("username", user.SetGetUsername);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                CloseConnection();
                return count > 0; // Gibt true zurück, wenn der Benutzer existiert
            }
        }

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

        public void insertIntoDatabase(string user, string pwd)
        {

            if (UserExists(user))
            {
                Console.WriteLine("Benutzer existiert bereits.");
                return; // Benutzername ist bereits vergeben
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

        public void updateDecksValues(User user, int id, string olduser)
        {
            // Open the connection
            OpenConnection();

            try
            {


                // Update the deck table
                using (var cmd3 = new NpgsqlCommand("UPDATE deck SET username = @username WHERE username = @olduser", connection))
                {
                    cmd3.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd3.Parameters.AddWithValue("olduser", olduser);

                    cmd3.ExecuteNonQuery();
                }
            }
            finally
            {
                // Ensure the connection is closed
                CloseConnection();
            }
        }

        public void updateCardsValues(User user, int id, string olduser)
        {
            // Open the connection
            OpenConnection();

            try
            {

                // Update the cards table
                using (var cmd2 = new NpgsqlCommand("UPDATE cards SET \"user\" = @username WHERE \"user\" = @olduser", connection))
                {
                    cmd2.Parameters.AddWithValue("username", user.SetGetUsername);
                    cmd2.Parameters.AddWithValue("olduser", olduser);

                    cmd2.ExecuteNonQuery();
                }

            }
            finally
            {
                // Ensure the connection is closed
                CloseConnection();
            }
        }

        public void updateUserValues(User user, int id, string olduser)
        {
            // Open the connection
            OpenConnection();

            try
            {
                // Update the users table
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
                // Ensure the connection is closed
                CloseConnection();
            }
        }


    }
}
