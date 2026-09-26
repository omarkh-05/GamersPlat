using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionParticipantBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(SessionParticipant sp)
        {
            int spID = await SessionParticipantDLL.Add(sp);
            return spID > 0;
        }
        public async Task<bool> Delete(int id) => await SessionParticipantDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<SessionParticipant?> GetBySessionId(int sessionId) => await SessionParticipantDLL.GetBySessionId(sessionId);
        // ================ Read By ================
    }
}
