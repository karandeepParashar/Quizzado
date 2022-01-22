using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Services
{
    public interface IUserService
    {
        User RegisterUser(User user);
        User Login(string userId, string password);
        bool VerifyOtp(string userid, int otp);
        User ResendOtp(string email);
        User Forgotpassword(string email);
        User ReferralService(User user);
    }
}
