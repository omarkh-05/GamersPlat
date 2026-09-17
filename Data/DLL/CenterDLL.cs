using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using Data.DLL;
using Domain.DTOs.Center;

namespace DataLayer
{
    public class CenterDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Center center)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Centers.Add(center);
                await db.SaveChangesAsync();
                return center.CenterId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Center Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Center center,int ownerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == center.CenterId && c.OwnerUserId == ownerId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(center);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Center Error", ex);
                return false;
            }
        }
        public static async Task<bool> UpdateActiveStatus(int centerId, int ownerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();

                var center = await db.Centers
                    .FirstOrDefaultAsync(c => c.CenterId == centerId && c.OwnerUserId == ownerId);

                if (center == null)
                    return false;

                center.IsActive = !center.IsActive;

                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Center Active Status Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == centerId);
                if (existing == null) return false;
                db.Centers.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Center Error", ex);
                return false;
            }
        }
        public static async Task<List<Center>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                    .Include(c => c.City)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Centers Error", ex);
                return new List<Center>();
            }
        }
        public static async Task<List<string>> GetCenterNames()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                    .Where(c => c.IsActive == true)
                    .Select(c => c.CenterName)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center Names Error", ex);
                return new List<string>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<Center?> GetByID(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                    .Include(c => c.City)
                    .Include(c => c.OwnerUser)
                    .Include(c => c.CenterImage)
                    .Include(c => c.Services)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center By ID Error", ex);
                return null;
            }
        }
        public static async Task<List<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                .Where(c => c.OwnerUserId == ownerId)
                .Select(c => new DTO_CentersListDetails
                {
                    CenterName = c.CenterName,
                    CenterAddress = c.CenterAddress,
                    CenterDescription = c.CenterDescription,
                    CenterStatus = c.CenterStatus,
                    CenterType = c.CenterType,
                    OpenTime = c.OpenTime,
                    CloseTime = c.CloseTime,
                    CityName = c.City.Name,
                    Rating = c.Reviews
                        .Select(r => (decimal?)r.Rating)
                        .Average() ?? 0,
                    ResourcesCount = c.ResourcesTypes
                        .Count(r => r.IsActive),
                    ServicesList = c.Services
                        .Where(s => s.IsActive)
                        .Select(s => s.Name)
                        .ToList()
                })
                .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Centers By Owner ID Error", ex);
                return new List<DTO_CentersListDetails>();
            }
        }
        // ================ Read By ================


        // ================ Other Methods ================
        public static async Task<bool> ToggleCenterStatus(int centerId,string status,int ownerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var center = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == centerId && c.OwnerUserId == ownerId);
                if (center == null) return false;
                center.CenterStatus = status;
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Toggle Center Status Error", ex);
                return false;
            }
        }
        // ================ Other Methods ================
    }
}
