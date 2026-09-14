using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterInvitationBLL
    {
        // ================ CRUD ================
        public bool Add(CenterInvitation centerInvitation)
        {
            int invID = CenterInvitationDLL.Add(centerInvitation);
            return invID > 0;
        }
        public bool Update(CenterInvitation centerInvitation) => CenterInvitationDLL.Update(centerInvitation);
        public bool Delete(int id) => CenterInvitationDLL.Delete(id);
        public static Task<List<CenterInvitation>> GetAll() => CenterInvitationDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<CenterInvitation?> GetByID(int id) => CenterInvitationDLL.GetByID(id);
        // ================ Read By ================
    }
}
