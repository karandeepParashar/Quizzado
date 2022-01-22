using LeaderBoard.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderBoard.Service
{
    public interface IUserService
    {
        long UpdateCoins(string Email, long coin);
        User RegisterUser(User user);
        User GetUserDetails(string email);
        User UpdateUserDetails(User user);
        bool AddScore(string Email, long categoryId, long categoryScore);
        List<Leaderboard> GetLeaderBoard(long categoryId);
        bool InsertNewCategory(long categoryId);
        long? GetRank(string Email, long categoryId);
    }
}
