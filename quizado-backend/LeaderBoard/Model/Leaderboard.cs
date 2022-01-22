using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderBoard.Model
{
    public class Leaderboard
    {
        public int Rank { get; set; }
        public CategoryScore CategoryScore { get; set; }
        public string Email { get; set; }
    }
}
