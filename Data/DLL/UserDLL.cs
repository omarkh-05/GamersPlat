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
        private readonly GamersPlatDbContext _db;

        public UserDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(User user)
        {
            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return user.UserId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add User Error", ex);
                return 0;
            }
        }
        public async Task<bool> Update(int userId, User updateUserInfoRequest)
        {
            try
            {
                var existing = await _db.Users.FindAsync(userId);
                if (existing == null) return false;
                _db.Entry(existing).CurrentValues.SetValues(updateUserInfoRequest);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update User Error", ex);
                return false;
            }
        }
        public async Task<bool> Delete(int userId)
        {
            try
            {
                var existing = await _db.Users.FindAsync(userId);
                if (existing == null) return false;
                _db.Users.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete User Error", ex);
                return false;
            }
        }
        public async Task<List<DTO_UserListResponse>> GetAll()
        {
            try
            {
                return await _db.Users
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
        public async Task<User?> GetById(int userId)
        {
            try
            {
                return await _db.Users
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
        public async Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber)) return null;
                return await _db.Users
                    .Where(u => u.PhoneNumber == phoneNumber)
                        .Select(u => new DTO_UserInfoRequest
                        {
                            PhoneNumber = u.PhoneNumber,
                            FullName = u.FullName,
                            Email = u.Email,
                            CityId = u.CityId,
                            Points = u.Points,
                            IsActive = u.IsActive,
                            Role = u.UserRoles.Select(ur => ur.Role.RoleName).FirstOrDefault() ?? "",
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
        public async Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId)
        {
            try
            {
                return await _db.Users
          .Where(u => u.UserId == userId)
          .Select(u => new DTO_UserInfoRequest
          {
              PhoneNumber = u.PhoneNumber,
              FullName = u.FullName,
              Email = u.Email,
              CityId = u.CityId,
              Points = u.Points,
              IsActive = u.IsActive,
              Role = u.UserRoles.Select(ur => ur.Role.RoleName).FirstOrDefault() ?? "",
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
        public async Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass)
        {
            try
            {
                return await _db.Users
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
        public async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return false;
                return await _db.Users.AnyAsync(u => u.Email == email && (excludeId == 0 || u.UserId != excludeId));
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Exists By Email Error", ex);
                return false;
            }
        }
        public async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone)) return false;
                return await _db.Users.AnyAsync(u => u.PhoneNumber == phone && (excludeId == 0 || u.UserId != excludeId));
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
