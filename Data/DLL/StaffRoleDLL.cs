using Data.EF;
using Domain.DTOs.Staff;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public class StaffRoleDLL
    {
        private readonly GamersPlatDbContext _db;

        public StaffRoleDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ Role Management ================
        public async Task<bool> AddStaffRole(StaffRole role)
        {
            try
            {
                _db.StaffRoles.Add(role);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Staff Role Error", ex);
                return false;
            }
        }
        public async Task<bool> SetRoleToMember(Staff role)
        {
            try
            {
                var staff = await _db.Staff
                    .FirstOrDefaultAsync(s => s.StaffId == role.StaffId);
                if (staff == null)
                    return false;
                var roleExists = await _db.StaffRoles
                    .AnyAsync(r => r.StaffRoleId == role.StaffRoleId);
                if (!roleExists)
                    return false;
                staff.StaffRoleId = role.StaffRoleId;
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Set Staff Role Error", ex);
                return false;
            }
        }
        // ================ Role Management ================
    }
}