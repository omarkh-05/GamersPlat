using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterImageBLL
    {
        // ================ CRUD ================
        public bool Add(CenterImage image)
        {
            int imageID = CenterImageDLL.Add(image);
            return imageID > 0;
        }
        public bool Update(CenterImage image) => CenterImageDLL.Update(image);
        public bool Delete(int id) => CenterImageDLL.Delete(id);
        public static Task<List<CenterImage>> GetAll() => CenterImageDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<CenterImage?> GetByID(int id) => CenterImageDLL.GetByID(id);
        // ================ Read By ================
    }
}
