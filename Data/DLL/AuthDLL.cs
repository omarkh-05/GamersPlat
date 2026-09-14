using Data.EF;
using DataLayer;
using Domain.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Data.DLL
{
    public class AuthDLL
    {
        // ================ Read By For Auth ================
        public static async Task<DTO_UserAuthInfo?> GetUserAuthByPhone(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber)) return null;
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .Where(u => u.PhoneNumber == phoneNumber)
                        .Select(u => new DTO_UserAuthInfo
                        {
                            UserId = u.UserId,
                            PasswordHash = u.PasswordHash,
                            PhoneNumber = u.PhoneNumber,
                            Email = u.Email,
                        })
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User Auth By Phone Error", ex);
                return null;
            }
        }
        public static async Task<DTO_UserAuthInfo?> GetUserAuthByID(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
          .Where(u => u.UserId == userId)
          .Select(u => new DTO_UserAuthInfo
          {
              UserId = u.UserId,
              PasswordHash = u.PasswordHash,
              PhoneNumber = u.PhoneNumber,
              Email = u.Email,
          })
          .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User Auth By ID Error", ex);
                return null;
            }
        }
        // ================ Read By For Auth ================


        // ================ Security ================
        public static async Task<bool> ChangePassword(int userId, string newPasswordHash)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FindAsync(userId);
                if (existing == null) return false;
                existing.PasswordHash = newPasswordHash;
                existing.UpdatedAt = DateTime.UtcNow;
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Password Error", ex);
                return false;
            }
        }
        public static async Task<bool> VerifyEmail(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FindAsync(userId);
                if (existing == null) return false;
                existing.EmailVerified = true;
                existing.UpdatedAt = DateTime.UtcNow;
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Verify Email Error", ex);
                return false;
            }
        }
        public static async Task<bool> VerifyPhone(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FindAsync(userId);
                if (existing == null) return false;
                existing.PhoneVerified = true;
                existing.UpdatedAt = DateTime.UtcNow;
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Verify Phone Error", ex);
                return false;
            }
        }
        // ================ Security ================


    }
}
