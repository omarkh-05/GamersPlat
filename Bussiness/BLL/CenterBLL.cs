using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterBLL
    {
        public int _centerID { get; private set; }
        // ================ CRUD ================
        public async Task<bool> Add(Center center)
        {
            int centerID = await CenterDLL.Add(center);
            _centerID = centerID;
            return centerID > 0;
        }
        public async Task<bool> Update(Center center, int ownerId) => await CenterDLL.Update(center, ownerId);
        public async Task<bool> UpdateActiveStatus(int centerId, int ownerId) => await CenterDLL.UpdateActiveStatus(centerId, ownerId);
        public async Task<bool> ToggleCenterStatus(int centerId, string status, int ownerId) => await CenterDLL.ToggleCenterStatus(centerId, status, ownerId);
        public async Task<bool> Delete(int centerId) => await CenterDLL.Delete(centerId);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Center?> GetByID(int centerId) {
            if(centerId <= 0) throw new ArgumentException("Invalid center ID.");
            return await CenterDLL.GetByID(centerId);
        }
        public async Task<List<Center>> GetAll() => await CenterDLL.GetAll();
        public async Task<List<string>> GetCenterNames() => await CenterDLL.GetCenterNames();
        // ================ Read By ================
    }
}
