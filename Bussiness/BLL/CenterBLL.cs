using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterBLL
    {
        // ================ CRUD ================
        public bool Add(Center center)
        {
            int centerID = CenterDLL.Add(center);
            return centerID > 0;
        }
        public async Task<bool> Update(Center center) => await CenterDLL.Update(center);
        public async Task<bool> Delete(int centerId) => await CenterDLL.Delete(centerId);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Center?> GetByID(int centerId) => await CenterDLL.GetByID(centerId);
        public async Task<List<Center>> GetAll() => await CenterDLL.GetAll();
        public async Task<List<string>> GetCenterNames() => await CenterDLL.GetCenterNames();
        // ================ Read By ================
    }
}
