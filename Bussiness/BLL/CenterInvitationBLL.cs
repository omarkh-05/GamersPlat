using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterInvitationBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(CenterInvitation centerInvitation)
        {
            int invID = await CenterInvitationDLL.Add(centerInvitation);
            return invID > 0;
        }
        public async Task<bool> Update(CenterInvitation centerInvitation) => await CenterInvitationDLL.Update(centerInvitation);
        public async Task<bool> Delete(int id) => await CenterInvitationDLL.Delete(id);
        public async Task<List<CenterInvitation>> GetAll() => await CenterInvitationDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterInvitation?> GetByID(int id) => await CenterInvitationDLL.GetByID(id);
        // ================ Read By ================
    }
}
