using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class UserDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(User user)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return user.UserId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add User Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(User user)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FindAsync(user.UserId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(user);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update User Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FindAsync(userId);
                if (existing == null) return false;
                db.Users.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete User Error", ex);
                return false;
            }
        }
        public static async Task<List<DTO_UserListResponse>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .AsNoTracking()
            .Select(u => new DTO_UserListResponse
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                CityId = u.CityId,
                Points = u.Points,
                IsActive = u.IsActive,
                PhoneVerified = u.PhoneVerified == false ? false : true,
                EmailVerified = u.EmailVerified == false ? false : true,

                Roles = u.UserRoles
                    .Select(ur => ur.Role.RoleName)
                    .ToList()
            })
            .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Users Error", ex);
                return new List<DTO_UserListResponse>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<User?> GetById(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .Include(u => u.UserRoles!)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User By ID Error", ex);
                return null;
            }
        }
        public static async Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber)) return null;
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .Where(u => u.PhoneNumber == phoneNumber)
                        .Select(u => new DTO_UserInfoRequest
                        {
                            PhoneNumber = u.PhoneNumber,
                            FullName = u.FullName,
                            Email = u.Email,
                            CityId = u.CityId,
                            Points = u.Points,
                            IsActive = u.IsActive,
                            PhoneVerified = u.PhoneVerified == false ? false : true,
                            EmailVerified = u.EmailVerified == false ? false : true
                        })
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User By Phone Error", ex);
                return null;
            }
        }
        public static async Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
          .Where(u => u.UserId == userId)
          .Select(u => new DTO_UserInfoRequest
          {
              PhoneNumber = u.PhoneNumber,
              FullName = u.FullName,
              Email = u.Email,
              CityId = u.CityId,
              Points = u.Points,
              IsActive = u.IsActive,
              PhoneVerified = u.PhoneVerified == false ? false : true,
              EmailVerified = u.EmailVerified == false ? false : true
          })
          .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User By ID Error", ex);
                return null;
            }
        }
        public static async Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
                   .Where(u => u.Email == reqResetPass.Email || u.PhoneNumber == reqResetPass.PhoneNumber)
                    .Select(u => u.UserId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User By Phone Or Email ID Error", ex);
                return -1;
            }
        }
        // ================ Read By ================


        // ================ Validation ============
        public static async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return false;
                using var db = new GamersPlatDbContext();
                return await db.Users.AnyAsync(u => u.Email == email && (excludeId == 0 || u.UserId != excludeId));
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Exists By Email Error", ex);
                return false;
            }
        }
        public static async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone)) return false;
                using var db = new GamersPlatDbContext();
                return await db.Users.AnyAsync(u => u.PhoneNumber == phone && (excludeId == 0 || u.UserId != excludeId));
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Exists By Phone Error", ex);
                return false;
            }
        }
        // ================ Validation ============
    }
}
