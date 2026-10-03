using Data.EF;
using Domain.DTOs.Staff;
using Microsoft.EntityFrameworkCore;

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
        public async Task<int> Add(DTO_AddStaffMember staff)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                User user = new User
                {
                    FullName = staff.FullName,
                    PhoneNumber = staff.PhoneNumber,
                    PasswordHash = staff.PasswordHash,
                    Email = staff.Email,
                    CityId = staff.CityId
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                Staff staffEntity = new Staff
                {
                    UserId = user.UserId,
                    CenterId = staff.CenterId,
                    StaffRoleId = staff.StaffRoleId,
                    IsActive = staff.IsActive
                };

                _db.Staff.Add(staffEntity);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return staffEntity.StaffId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                EventLog_Helper.WriteEventLog("Add Staff Error", ex);
                return 0;
            }
        }
        public async Task<bool> Delete(int staffId)
        {
            try
            {
                var existing = await _db.Staff
                    .FirstOrDefaultAsync(s => s.StaffId == staffId);

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
                        FullName = s.User.FullName,
                        PhoneNumber = s.User.PhoneNumber,
                        CenterName = s.Center.CenterName,
                        StaffRoleName = s.StaffRole.Name,
                        Permissions = s.StaffRole.Permissions,
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
        public async Task<DTO_StaffDetails?> GetByID(int staffId)
        {
            try
            {
                return await _db.Staff
                    .Where(s => s.StaffId == staffId)
                    .Select(s => new DTO_StaffDetails
                    {
                        FullName = s.User.FullName,
                        PhoneNumber = s.User.PhoneNumber,
                        CenterName = s.Center.CenterName,
                        StaffRoleName = s.StaffRole.Name,
                        Permissions = s.StaffRole.Permissions,
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
                        FullName = s.User.FullName,
                        PhoneNumber = s.User.PhoneNumber,
                        CenterName = s.Center.CenterName,
                        StaffRoleName = s.StaffRole.Name,
                        Permissions = s.StaffRole.Permissions,
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
