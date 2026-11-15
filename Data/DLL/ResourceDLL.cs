using Data.EF;
using Domain.DTOs.Resource;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public class ResourceDLL
    {
        private readonly GamersPlatDbContext _db;

        public ResourceDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<List<DTO_Resources>> GetAll()
        {
            try
            {
                return await _db.Resources
                    .AsNoTracking()
                    .Select(x => new DTO_Resources
                    {
                        ResourcesTypeId = x.ResourcesTypeId,
                        ResourceName = x.ResourceName,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Services Error", ex);
                return new List<DTO_Resources>();
            }
        }
        // ================ CRUD ================
    }
}
