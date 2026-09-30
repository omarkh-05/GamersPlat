using Microsoft.EntityFrameworkCore;
using Data;
using Data.DLL;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class UserRoleDLL
    {
        private readonly GamersPlatDbContext _db;

        public UserRoleDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ================
        public async Task<int> Add(UserRole ur)
        {

            try
            {
                _db.UserRoles.Add(ur);
                await _db.SaveChangesAsync();
                return ur.Id;
            }
            catch (Exception ex)
            {
               EventLog_Helper.WriteEventLog("Add UserRole Error", ex);
                return 0;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<UserRole>> GetByUserId(int userId)
        {
            try
            {
                return await _db.UserRoles.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get UserRoles By User Error", ex);
                return new List<UserRole>();
            }
        }
        // ================ Read By ================
    }
}
