using UserClasses;
using CardsClasses;
using DatabaseClassses;
using System.Text;

namespace BattleClasses
{
    public class Battle
    {
        private Database _database;
        private Warteschlange _warteschlange;
        private List<Warteschlange> waitinglist;

        public Battle()
        {
            _database = new Database();
            _warteschlange = new Warteschlange();
            waitinglist = new List<Warteschlange>();
        }

        


        public int calcNormalBattle(Cards user1Card, Cards user2Card)
        {
            float damageUser1Card = user1Card.SetGetDamage;
            float damageUser2Card = user2Card.SetGetDamage;
            if (user1Card.SetGetCardType == 0)
            {
                if (user1Card.SetGetFamily == "Kraken" && user2Card.SetGetCardType == 1)
                {
                    damageUser2Card = 0;
                }
                if (user1Card.SetGetFamily == "Knight" && user2Card.SetGetCardType == 1)
                {
                    damageUser1Card = 0;
                }
            }
            if (user2Card.SetGetCardType == 0)
            {
                if (user2Card.SetGetFamily == "Kraken" && user1Card.SetGetCardType == 1)
                {
                    damageUser1Card = 0;
                }
                if (user2Card.SetGetFamily == "Knight" && user1Card.SetGetCardType == 1)
                {
                    damageUser2Card = 0;
                }
            }
            

            switch (user1Card.SetGetElement)
            {
                case "Fire":
                    if (user2Card.SetGetElement == "Water")
                    {
                        damageUser1Card = damageUser1Card / 2;
                        damageUser2Card = damageUser2Card * 2;
                    }
                    if(user2Card.SetGetElement == "Normal")
                    {
                        damageUser1Card = damageUser1Card * 2;
                        damageUser2Card = damageUser2Card / 2;
                    }
                    break;
                case "Water":
                    if (user2Card.SetGetElement == "Normal")
                    {
                        damageUser1Card = damageUser1Card / 2;
                        damageUser2Card = damageUser2Card * 2;
                    }
                    if (user2Card.SetGetElement == "Fire")
                    {
                        damageUser1Card = damageUser1Card * 2;
                        damageUser2Card = damageUser2Card / 2;
                    }
                    break;
                case "Normal":
                    if (user2Card.SetGetElement == "Fire")
                    {
                        damageUser1Card = damageUser1Card / 2;
                        damageUser2Card = damageUser2Card * 2;
                    }
                    if (user2Card.SetGetElement == "Water")
                    {
                        damageUser1Card = damageUser1Card * 2;
                        damageUser2Card = damageUser2Card / 2;
                    }
                    break;
            }

            if (user1Card.winCount > 3 && user1Card.winCount < 5)
            {
                damageUser1Card += 20;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(user1Card.SetGetName + " Got strenght through his previous wins");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (user2Card.winCount > 3 && user2Card.winCount < 5)
            {
                damageUser2Card += 20;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(user2Card.SetGetName + " Got strenght through his previous wins");
                Console.ForegroundColor = ConsoleColor.White;
            }


            if (damageUser1Card > damageUser2Card)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(user1Card.SetGetName + " won the battle (" + damageUser1Card + ")");
                Console.ForegroundColor = ConsoleColor.White;
                return 1;
            }
            else if (damageUser2Card > damageUser1Card)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(user2Card.SetGetName + " won the battle (" + damageUser2Card + ")");
                Console.ForegroundColor = ConsoleColor.White;
                return 2;
            }
            else
            {
                Console.WriteLine("Noone Won");
                return 0;
            }
        }

        public int CalcMonsterBattle(Cards user1Card, Cards user2Card)
        {
            float damageUser1Card = user1Card.SetGetDamage;
            float damageUser2Card = user2Card.SetGetDamage;
            switch (user1Card.SetGetFamily)
            {
                case "Goblin":
                    if (user2Card.SetGetFamily == "Dragon")
                    {
                        damageUser1Card = 0;
                    }
                    break;
                case "Dragon":
                    if (user2Card.SetGetFamily == "Goblin")
                    {
                        damageUser2Card = 0;
                    }
                    if ((user2Card.SetGetElement == "Fire") && (user2Card.SetGetFamily == "Elve"))
                    {
                        damageUser1Card = 0;
                    }
                    break;
                case "Wizzard":
                    if (user2Card.SetGetFamily == "Ork")
                    {
                        damageUser2Card = 0;
                    }
                    break;
                case "Ork":
                    if (user2Card.SetGetFamily == "Wizzard")
                    {
                        damageUser1Card = 0;
                    }
                    break;
                case "Knight":
                    break;
                case "Kraken":
                    break;
                case "Elve":
                    if ((user1Card.SetGetElement == "Fire") && (user2Card.SetGetFamily == "Dragon"))
                    {
                        damageUser2Card = 0;
                    }
                    break;
                default:
                    Console.WriteLine("Error in Monster Battle");
                    break;

            }

            if (user1Card.winCount > 3 && user1Card.winCount < 5)
            {
                damageUser1Card += 20;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(user1Card.SetGetName + " Got strenght through his previous wins");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (user2Card.winCount > 3 && user2Card.winCount < 5)
            {
                damageUser2Card += 20;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(user2Card.SetGetName + " Got strenght through his previous wins");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (damageUser1Card > damageUser2Card)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(user1Card.SetGetName + " won the battle (" + damageUser1Card + ")");
                Console.ForegroundColor = ConsoleColor.White;
                return 1;
            }
            else if (damageUser2Card > damageUser1Card)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(user2Card.SetGetName + " won the battle (" + damageUser2Card + ")");
                Console.ForegroundColor = ConsoleColor.White;
                return 2;
            }
            else
            {
                Console.WriteLine("Noone Won");
                return 0;
            }

        }

        public int BattleLogic(User user1, User user2)
        {
            List<Cards> user1DeckTemp = user1.SetGetCardsDeck;
            List<Cards> user2DeckTemp = user2.SetGetCardsDeck;

            for (int i = 0; i < 100; i++)
            {
                if(user1DeckTemp.Count() == 0 || user2DeckTemp.Count() == 0)
                {
                    if (user1DeckTemp.Count() > user2DeckTemp.Count())
                    {
                        Console.WriteLine(user1.SetGetUsername + " won the Game!");
                   
                        return 1;
                    }
                    else if (user2DeckTemp.Count() > user1DeckTemp.Count())
                    {
                        Console.WriteLine(user2.SetGetUsername + " won the Game!");
                       
                        return 2;
                    }
                    else
                    {
                        Console.WriteLine("No one Won the Round its a tie!");
                       
                        return 0;
                    }
                }

                if (user1DeckTemp.Count() > 0 && user2DeckTemp.Count() > 0)
                {
                    if ((user1DeckTemp[i % user1DeckTemp.Count()].SetGetCardType == 0) && (user2DeckTemp[i % user2DeckTemp.Count()].SetGetCardType == 0))
                    {
                        int j = CalcMonsterBattle(user1DeckTemp[i % user1DeckTemp.Count()], user2DeckTemp[i % user2DeckTemp.Count()]);
                        switch (j)
                        {
                            case 0:
                                break;
                            case 1:
                                if (user1DeckTemp.Count() == 0) { return 1; }
                                if (user2DeckTemp.Count() == 0) { return 2; }
                                user1DeckTemp[i % user1DeckTemp.Count()].winCount++;
                                user2DeckTemp[i % user2DeckTemp.Count()].winCount = 0;
                                user1DeckTemp.Add(user2DeckTemp[i % user2DeckTemp.Count()]);
                                user2DeckTemp.RemoveAt(i % user2DeckTemp.Count());
                                break;
                            case 2:
                                if (user1DeckTemp.Count() == 0) { return 1; }
                                if (user2DeckTemp.Count() == 0) { return 2; }
                                user2DeckTemp[i % user2DeckTemp.Count()].winCount++;
                                user1DeckTemp[i % user1DeckTemp.Count()].winCount = 0;
                                user2DeckTemp.Add(user1DeckTemp[i % user1DeckTemp.Count()]);
                                user1DeckTemp.RemoveAt(i % user1DeckTemp.Count());
                                break;
                        }

                    }
                    else
                    {
                        int j = calcNormalBattle(user1DeckTemp[i % user1DeckTemp.Count()], user2DeckTemp[i % user2DeckTemp.Count()]);
                        switch (j)
                        {
                            case 0:
                                break;
                            case 1:
                                if(user1DeckTemp.Count() == 0) { return 1; }
                                if(user2DeckTemp.Count() == 0) { return 2; }
                                user1DeckTemp[i % user1DeckTemp.Count()].winCount++;
                                user2DeckTemp[i % user2DeckTemp.Count()].winCount = 0;
                                user1DeckTemp.Add(user2DeckTemp[i % user2DeckTemp.Count()]);
                                user2DeckTemp.RemoveAt(i % user2DeckTemp.Count());
                                break;
                            case 2:
                                if (user1DeckTemp.Count() == 0) { return 1; }
                                if (user2DeckTemp.Count() == 0) { return 2; }
                                user2DeckTemp[i % user2DeckTemp.Count()].winCount++;
                                user1DeckTemp[i % user1DeckTemp.Count()].winCount = 0;
                                user2DeckTemp.Add(user1DeckTemp[i % user1DeckTemp.Count()]);
                                user1DeckTemp.RemoveAt(i % user1DeckTemp.Count());
                                break;
                        }
                    }
                }
                else
                {
                    if (user1DeckTemp.Count() > user2DeckTemp.Count())
                    {
                        Console.WriteLine(user1.SetGetUsername + " won the Game!");
                        
                    }
                    else if (user2DeckTemp.Count() > user1DeckTemp.Count())
                    {
                        Console.WriteLine(user2.SetGetUsername + " won the Game!");
                        
                    }
                }

                if (i == 99)
                {
                    Console.WriteLine("Max Rounds Reached");
                    if (user1DeckTemp.Count() > user2DeckTemp.Count())
                    {
                        Console.WriteLine(user1.SetGetUsername + " won the Game!");
                       
                        return 1;
                    }
                    else if (user2DeckTemp.Count() > user1DeckTemp.Count())
                    {
                        Console.WriteLine(user2.SetGetUsername + " won the Game!");
                        
                        return 2;
                    }
                    else
                    {
                        Console.WriteLine("No one Won the Round its a tie!");
                       
                        return 0;
                    }
                }
            }
            return 0;
        }

        public (User, User) StartBattle(User user1, User user2)
        {
            Console.WriteLine("Battle start");

            int j = BattleLogic(user1, user2);

            if (j == 1)
            {
                user1.SetGetWins++;
                user2.SetGetLose++;
            }
            else if (j == 2)
            {
                user2.SetGetWins++;
                user1.SetGetLose++;
            }

            return (user1, user2);
        }

        public (User, User) CalculateBattleList(User user, List<User> users)
        {
            //_database.AddToBattleQueue(user.SetGetUsername);


            // Alle Benutzer aus der Warteschlange holen
            List<Warteschlange> battleQueue = _database.GetBattleQueue(user.SetGetUsername);
            // battlequeue wird momentan nicht benutzt weil ich die warteschlange nicht hinbekommen habe

            Warteschlange firstPlayer = new Warteschlange();
            firstPlayer.setGetUser1Name = "Altenhofer";
            Warteschlange secondPlayer = new Warteschlange();
            secondPlayer.setGetUser1Name = "Kienboeck";

            /*
            // Suche nach zwei Spielern, die bereit sind
            foreach (var entry in battleQueue)
            {
                if (entry.setGetUser1Name != string.Empty && entry.setGetUser2Name == string.Empty)
                {
                    if (firstPlayer == null)
                    {
                        firstPlayer = entry;
                        break;
                    }
                }
            }

            // Wenn zwei Spieler in der Warteschlange sind
            if (firstPlayer != null && secondPlayer == null)
            {
                foreach (var entry in battleQueue)
                {
                    if (entry.setGetUser1Name == string.Empty && entry.setGetUser2Name == string.Empty)
                    {
                        secondPlayer = entry;
                        break;
                    }
                }
            }
            */

            if (firstPlayer != null && secondPlayer != null)
            {
                Console.WriteLine($"Kampf zwischen {firstPlayer.setGetUser1Name} und {secondPlayer.setGetUser1Name} wurde gestartet.");
                User user1 = users.FirstOrDefault(j => j != null && j.SetGetUsername == firstPlayer.setGetUser1Name);
                User user2 = users.FirstOrDefault(j => j != null && j.SetGetUsername == secondPlayer.setGetUser1Name);
                // Kämpfe starten und entferne beide Spieler aus der Warteschlange
                var usersNew = new Battle().StartBattle(user1, user2);
                //_database.RemoveFromBattleQueue(firstPlayer);
                //_database.RemoveFromBattleQueue(secondPlayer);
                
          
                return usersNew;
            }

            return (null, null);
        }
        /*

        public (User, User) calculateBattleList(User user)
        {
            foreach (var entry in WaitingListBattle)
            {
                if (entry.Key.SetGetUsername == user.SetGetUsername || entry.Value?.SetGetUsername == user.SetGetUsername)
                {
                    Console.WriteLine($"{user.SetGetUsername} ist bereits in der Warteliste.");
                    return (null, null);
                }
            }

            if (WaitingListBattle.Count == 0)
            {
                WaitingListBattle.Add(user, null);
                Console.WriteLine($"{user.SetGetUsername} wurde der Warteliste hinzugefügt.");
            }
            else
            {
                foreach (var entry in WaitingListBattle)
                {
                    if (entry.Value == null)
                    {
                        WaitingListBattle[entry.Key] = user;
                        Console.WriteLine($"Battle zwischen {entry.Key.SetGetUsername} und {user.SetGetUsername}");

                        var usersNew = new Battle().StartBattle(entry.Key, user);

                        WaitingListBattle.Remove(entry.Key);
                        return usersNew;
                    }
                }
            }

            return (null, null);
        }

        */

        public void printLeaderboard(List<User> users)
        {
            IEnumerable<User> query = users.OrderByDescending(t => t.SetGetElo);

            int i = 0;
            foreach (var user in query)
            {
                i++;
                Console.WriteLine("{0}: {1}: {2}", i, user.SetGetUsername, user.SetGetElo);
            }

            return;
        }
    }
}
