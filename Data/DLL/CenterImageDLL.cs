using Microsoft.EntityFrameworkCore;
using Data;
using Data.DLL;
using Data.EF;

namespace DataLayer
{
    public class CenterImageDLL
    {
        private readonly GamersPlatDbContext _db;

        public CenterImageDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(CenterImage img)
        {
            try
            {
                _db.CenterImages.Add(img);
                await _db.SaveChangesAsync();
                return img.ImageId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add CenterImage Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(CenterImage img)
        {
            try
            {
                var existing = await _db.CenterImages
                    .FirstOrDefaultAsync(i => i.ImageId == img.ImageId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(img);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update CenterImage Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.CenterImages
                    .FirstOrDefaultAsync(i => i.ImageId == id);

                if (existing == null)
                    return false;

                _db.CenterImages.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete CenterImage Error", ex);
                return false;
            }
        }

        public async Task<List<CenterImage>> GetAll()
        {
            try
            {
                return await _db.CenterImages
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All CenterImages Error", ex);
                return new List<CenterImage>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterImage?> GetByID(int id)
        {
            try
            {
                return await _db.CenterImages
                    .Include(i => i.Center)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.ImageId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get CenterImage By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}