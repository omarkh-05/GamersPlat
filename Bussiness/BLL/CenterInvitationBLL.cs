using Data;
using DataLayer;

namespace Bussiness
{
    public class CenterInvitationBLL
    {
        private readonly CenterInvitationDLL _centerInvitationDLL;

        public CenterInvitationBLL(CenterInvitationDLL centerInvitationDLL)
        {
            _centerInvitationDLL = centerInvitationDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(CenterInvitation centerInvitation)
        {
            int invID = await _centerInvitationDLL.Add(centerInvitation);

            return invID > 0;
        }

        public async Task<bool> Update(CenterInvitation centerInvitation)
            => await _centerInvitationDLL.Update(centerInvitation);

        public async Task<bool> Delete(int id)
            => await _centerInvitationDLL.Delete(id);

        public async Task<List<CenterInvitation>> GetAll()
            => await _centerInvitationDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterInvitation?> GetByID(int id)
            => await _centerInvitationDLL.GetByID(id);
        // ================ Read By ================
    }
}