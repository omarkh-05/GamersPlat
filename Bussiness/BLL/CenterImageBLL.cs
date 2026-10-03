using Data;
using DataLayer;
using Bussiness.Interfaces;

namespace Bussiness
{
    public class CenterImageBLL : ICenterImageService
    {
        private readonly CenterImageDLL _centerImageDLL;

        public int _imageID { get; private set; }
        public int LastId => _imageID;

        public CenterImageBLL(CenterImageDLL centerImageDLL)
        {
            _centerImageDLL = centerImageDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(CenterImage image)
        {
            int imageID = await _centerImageDLL.Add(image);

            _imageID = imageID;

            return imageID > 0;
        }

        public async Task<bool> Update(CenterImage image)
            => await _centerImageDLL.Update(image);

        public async Task<bool> Delete(int id)
            => await _centerImageDLL.Delete(id);

        public async Task<List<CenterImage>> GetAll()
            => await _centerImageDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterImage?> GetByID(int id)
            => await _centerImageDLL.GetByID(id);
        // ================ Read By ================
    }
}