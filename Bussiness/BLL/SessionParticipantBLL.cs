using Data;
using DataLayer;

namespace Bussiness
{
    public class SessionParticipantBLL
    {
        private readonly SessionParticipantDLL _sessionParticipantDLL;

        public SessionParticipantBLL(SessionParticipantDLL sessionParticipantDLL)
        {
            _sessionParticipantDLL = sessionParticipantDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(SessionParticipant sp)
        {
            int spID = await _sessionParticipantDLL.Add(sp);
            return spID > 0;
        }

        public async Task<bool> Delete(int id)
            => await _sessionParticipantDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<SessionParticipant?> GetBySessionId(int sessionId)
            => await _sessionParticipantDLL.GetBySessionId(sessionId);
        // ================ Read By ================
    }
}