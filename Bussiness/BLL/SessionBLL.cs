using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionBLL
    {
        private readonly SessionDLL _sessionDLL;

        public SessionBLL(SessionDLL sessionDLL)
        {
            _sessionDLL = sessionDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Session session)
        {
            int sessionID = await _sessionDLL.Add(session);
            return sessionID > 0;
        }
        public async Task<bool> Update(Session session) => await _sessionDLL.Update(session);
        public async Task<bool> Delete(int sessionId) => await _sessionDLL.Delete(sessionId);
        public async Task<List<Session>> GetAll() => await _sessionDLL.GetAll();
        // ================ CRUD ================

        // ================ Read By ================
        public async Task<Session?> GetByID(int sessionId) => await _sessionDLL.GetByID(sessionId);
        // ================ Read By ================
    }
}
