using Data;
using DataLayer;
namespace Bussiness
{
    public class PointTransactionBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(PointTransaction pt)
        {
            int ptID = await PointTransactionDLL.Add(pt);
            return ptID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<PointTransaction>> GetByUserId(int userId) => await PointTransactionDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}
