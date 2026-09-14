using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionBLL
    {
        // ================ CRUD ================
        public bool Add(Session session)
        {
            int sessionID = SessionDLL.Add(session);
            return sessionID > 0;
        }
        public bool Update(Session session) => SessionDLL.Update(session);
        public bool Delete(int sessionId) => SessionDLL.Delete(sessionId);
        public Task<List<Session>> GetAll() => SessionDLL.GetAll();
        // ================ CRUD ================

        // ================ Read By ================
        public Task<Session?> GetByID(int sessionId) => SessionDLL.GetByID(sessionId);
        // ================ Read By ================
    }
}
