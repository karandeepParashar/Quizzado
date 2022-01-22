using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models
{
    public interface IUserRepository
    {
        User FindUserById(string UserId);
        User Register(User user);
        User Login(string userId, string password);
        bool VerifyMail(string userId, int otp);
        User UpdatePassword(User user);
        User Resend(User user);
        string ToSHA256(string value);
        User FindUserByReferral(User user);
        string GenerateReferral();
        void UpdateDb(User user);
    }
}