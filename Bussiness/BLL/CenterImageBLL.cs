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
        public async Task<bool> Update(CenterImage image) => await CenterImageDLL.Update(image);
        public async Task<bool> Delete(int id) => await CenterImageDLL.Delete(id);
        public async Task<List<CenterImage>> GetAll() => await CenterImageDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterImage?> GetByID(int id) => await CenterImageDLL.GetByID(id);
        // ================ Read By ================
    }
}
