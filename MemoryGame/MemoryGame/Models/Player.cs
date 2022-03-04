using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class Player
    {

        public string Username { get; set; }
        public int Score { get; set; }

        public Level Level{ get; set; }

        public Player()
        {
        }

        public Player(string username, int Score, Level level)
        {
            this.Username=username;
            this.Score = Score;
            this.Level = level;
        }

        public override string? ToString()
        {
            return Username + "#" + Score + "#" + (int)Level;
        }
    }
}
