using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class RoleDLL
    {
        //public static int Add(Role role)
        //{
        //    try
        //    {
        //        using var db = new GamersPlatDbContext();
        //        db.Roles.Add(role);
        //        db.SaveChanges();
        //        return role.RoleId;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteEventLog("Add Role Error", ex);
        //        return 0;
        //    }
        //}

        //public static bool Update(Role role)
        //{
        //    try
        //    {
        //        using var db = new GamersPlatDbContext();
        //        var existing = db.Roles.FirstOrDefault(r => r.RoleId == role.RoleId);
        //        if (existing == null) return false;
        //        db.Entry(existing).CurrentValues.SetValues(role);
        //        return db.SaveChanges() > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteEventLog("Update Role Error", ex);
        //        return false;
        //    }
        //}

        //public static bool Delete(int roleId)
        //{
        //    try
        //    {
        //        using var db = new GamersPlatDbContext();
        //        var existing = db.Roles.FirstOrDefault(r => r.RoleId == roleId);
        //        if (existing == null) return false;
        //        db.Roles.Remove(existing);
        //        return db.SaveChanges() > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteEventLog("Delete Role Error", ex);
        //        return false;
        //    }
        //}
        //public static bool ExistsByName(string? name, int excludeId = 0)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(name)) return false;
        //        using var db = new GamersPlatDbContext();
        //        return db.Roles.Any(r => r.RoleName == name && (excludeId == 0 || r.RoleId != excludeId));
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteEventLog("Exists Role By Name Error", ex);
        //        return false;
        //    }
        //}

        public static async Task<Role?> GetByID(int roleId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RoleId == roleId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Role By ID Error", ex);
                return null;
            }
        }

        public static async Task<Role?> GetByName(string roleName)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RoleName == roleName);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Role By Name Error", ex);
                return null;
            }
        }
        

    }
}
