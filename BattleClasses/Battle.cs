using UserClasses;
using CardsClasses;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace BattleClasses
{
    public class Battle
    {


        private static ConcurrentQueue<User> playerQueue = new ConcurrentQueue<User>();
        private static readonly object battlelock = new object();

        public Battle()
        {

        }

        private static int calcNormalBattle(Cards user1Card, Cards user2Card)
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

        private static int CalcMonsterBattle(Cards user1Card, Cards user2Card)
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

        public static int BattleLogic(User user1, User user2)
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

        public static (User, User) StartBattle(User user1, User user2)
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

        private static (User, User) CheckQueue()
        {
            
            lock (battlelock)
            {
                if (playerQueue.Count() >= 2)
                {
                     
                    
                    if (playerQueue.TryDequeue(out User player1) && playerQueue.TryDequeue(out User player2))
                    {
                        Console.WriteLine($"Kampf zwischen {player1.SetGetUsername} und {player2.SetGetUsername} wurde gestartet.");
                        return StartBattle(player1, player2);
                    }
                }
                return (null, null);
            }
        }

        public (User, User) CalculateBattleList(User user, List<User> users)
        {

            playerQueue.Enqueue(user);
            Console.WriteLine(user.SetGetUsername+ " ist in der warteschlange");
            return CheckQueue();
           // return (null,null);
        }
        
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
