using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClasses
{
    public class Warteschlange
    {

        private int warteID;
        private string user1Name;
        private string user2Name;

        public Warteschlange()
        {
            warteID = 0;
            user1Name = null;
            user2Name = null;
        }

        public int setGetWarteID
        {
            get => warteID; set => warteID = value;
        }
        public string setGetUser1Name 
        { 
            get => user1Name; set => user1Name = value;
        }

        public string setGetUser2Name
        {
            get => user2Name; set => user2Name = value;
        }
    }
}
