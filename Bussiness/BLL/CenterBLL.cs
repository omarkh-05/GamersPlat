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
        public bool Update(Center center) => CenterDLL.Update(center);
        public bool Delete(int centerId) => CenterDLL.Delete(centerId);
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Center?> GetByID(int centerId) => CenterDLL.GetByID(centerId);
        public static Task<List<Center>> GetAll() => CenterDLL.GetAll();
        public static Task<List<string>> GetCenterNames() => CenterDLL.GetCenterNames();
        // ================ Read By ================
    }
}
