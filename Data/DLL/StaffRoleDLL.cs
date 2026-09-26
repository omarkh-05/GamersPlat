using Data.EF;
using Domain.DTOs.Staff;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public class StaffRoleDLL
    {
        // ================ Role Management ================
        public static async Task<bool> AddStaffRole(StaffRole role)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.StaffRoles.Add(role);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Staff Role Error", ex);
                return false;
            }
        }
        public static async Task<bool> SetRoleToMember(Staff role)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var staff = await db.Staff
                    .FirstOrDefaultAsync(s => s.StaffId == role.StaffId);
                if (staff == null)
                    return false;
                var roleExists = await db.StaffRoles
                    .AnyAsync(r => r.StaffRoleId == role.StaffRoleId);
                if (!roleExists)
                    return false;
                staff.StaffRoleId = role.StaffRoleId;
                return await db.SaveChangesAsync() > 0;
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