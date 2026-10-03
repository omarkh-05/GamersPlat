using Data;
using DataLayer;

namespace Bussiness
{
    public class PointTransactionBLL
    {
        private readonly PointTransactionDLL _pointTransactionDLL;

        public PointTransactionBLL(PointTransactionDLL pointTransactionDLL)
        {
            _pointTransactionDLL = pointTransactionDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(PointTransaction pt)
        {
            int ptID = await _pointTransactionDLL.Add(pt);
            return ptID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<PointTransaction>> GetByUserId(int userId)
            => await _pointTransactionDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}