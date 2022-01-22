using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderBoard.Model
{
    public class User
    {
        public User()
        {
            Score = new List<CategoryScore>();
        }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public List<CategoryScore> Score { get; set; }
        public long Coins { get; set; }
    }
}
