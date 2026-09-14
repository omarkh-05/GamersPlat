using Data;
using DataLayer;
namespace Bussiness
{
    public class PointTransactionBLL
    {
        // ================ CRUD ================
        public bool Add(PointTransaction pt)
        {
            int ptID = PointTransactionDLL.Add(pt);
            return ptID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<List<PointTransaction>> GetByUserId(int userId) => PointTransactionDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}
