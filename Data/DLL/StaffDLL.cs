using Data.EF;
using Domain.DTOs.Staff;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Data.DLL
{
    public class StaffDLL
    {
        private readonly GamersPlatDbContext _db;

        public StaffDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ================
        public async Task<int> Add(Staff staff)
        {
            try
            {
                _db.Staff.Add(staff);
                await _db.SaveChangesAsync();

                return staff.StaffId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Staff Error", ex);
                return -1;
            }
        }
        // Todo: Add Updated By Who For Updated and Created By Who For Add
        public async Task<bool> Update(DTO_UpdateStaffMember staff)
        {
            try
            {
                var existing = await _db.Staff
                    .FirstOrDefaultAsync(s => s.StaffId == staff.StaffId);

                if (existing == null)
                    return false;

                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.UserId == existing.UserId);

                if (user == null)
                    return false;

                existing.CenterId = staff.CenterId;
                existing.Permissions = staff.Permissions;
                existing.RoleName = staff.StaffRoleName;
                existing.Status = staff.Status;
                existing.IsActive = staff.IsActive;

                user.FullName = staff.FullName;
                user.PhoneNumber = staff.PhoneNumber;
                user.Email = staff.Email;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Staff Error", ex);
                throw;
            }
        }
        public async Task<bool> Delete(int staffId)
        {
            try
            {
                var existing = await _db.Staff
                    .FirstOrDefaultAsync(s => s.UserId == staffId);

                if (existing == null)
                    return false;

                _db.Staff.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Staff Error", ex);
                return false;
            }
        }
        public async Task<List<DTO_StaffDetails>> GetAll()
        {
            try
            {
                return await _db.Staff
                    .Select(s => new DTO_StaffDetails
                    {
                        StaffId = s.StaffId,
                        FullName = s.User.FullName,
                        PhoneNumber = s.User.PhoneNumber,
                        CenterName = s.Center.CenterName,
                        StaffRoleName = s.RoleName ?? string.Empty,
                        Permissions = s.Permissions,
                        IsActive = s.IsActive
                    })
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Staff Error", ex);
                return new List<DTO_StaffDetails>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<DTO_StaffDetails?> GetByID(int userId)
        {
            try
            {
                return await _db.Staff
                    .Where(s => s.UserId == userId)
                    .Select(s => new DTO_StaffDetails
                    {
                        StaffId = s.StaffId,
                        CenterId = s.CenterId,
                        FullName = s.User.FullName,
                        PhoneNumber = s.User.PhoneNumber,
                        CenterName = s.Center.CenterName,
                        StaffRoleName = s.RoleName ?? string.Empty,
                        Permissions = s.Permissions,
                        Status = s.Status,
                        IsActive = s.IsActive
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Staff By ID Error", ex);
                return null;
            }
        }
        public async Task<List<DTO_StaffDetails>> GetByCenterId(int centerId)
        {
            try
            {
                return await _db.Staff
                    .Where(s => s.CenterId == centerId)
                    .Select(s => new DTO_StaffDetails
                    {
                        StaffId = s.StaffId,
                        FullName = s.User.FullName,
                        PhoneNumber = s.User.PhoneNumber,
                        CenterName = s.Center.CenterName,
                        StaffRoleName = s.RoleName ?? string.Empty,
                        Permissions = s.Permissions,
                        IsActive = s.IsActive
                    })
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Staff By Center ID Error", ex);
                return new List<DTO_StaffDetails>();
            }
        }
        // ================ Read By ================
    }
}
