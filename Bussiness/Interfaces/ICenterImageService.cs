using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface ICenterImageService
    {
        Task<bool> Add(CenterImage image);
        Task<bool> Update(CenterImage image);
        Task<bool> Delete(int id);
        Task<List<CenterImage>> GetAll();
        Task<CenterImage?> GetByID(int id);
        int LastId { get; }
    }
}
