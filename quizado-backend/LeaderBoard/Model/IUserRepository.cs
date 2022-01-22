using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderBoard.Model
{
    public interface IUserRepository
    {
        long UpdateCoins(string Email, long coin);
        bool GetUserByEmail(string Email);
        Boolean RegisterUser(User user);
        Dictionary<string, object> GetUserDetails(string email);
        Boolean UpdateUserDetails(User user);
        bool AddScore(string Email, long categoryId, long categoryScore);
        List<Leaderboard> GetLeaderBoard(long categoryId);
        bool InsertNewCategory(long categoryId);
        long? GetRank(string Email, long categoryId);
        bool GetCategory(long categoryId);
        List<string> GetCategoryList();
    }
}
