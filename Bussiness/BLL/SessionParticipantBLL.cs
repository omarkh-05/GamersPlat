using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionParticipantBLL
    {
        // ================ CRUD ================
        public bool Add(SessionParticipant sp)
        {
            int spID = SessionParticipantDLL.Add(sp);
            return spID > 0;
        }
        public bool Delete(int id) => SessionParticipantDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<SessionParticipant?> GetBySessionId(int sessionId) => SessionParticipantDLL.GetBySessionId(sessionId);
        // ================ Read By ================
    }
}
