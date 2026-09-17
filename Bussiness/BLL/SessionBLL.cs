using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Session session)
        {
            int sessionID = await SessionDLL.Add(session);
            return sessionID > 0;
        }
        public async Task<bool> Update(Session session) => await SessionDLL.Update(session);
        public async Task<bool> Delete(int sessionId) => await SessionDLL.Delete(sessionId);
        public async Task<List<Session>> GetAll() => await SessionDLL.GetAll();
        // ================ CRUD ================

        // ================ Read By ================
        public async Task<Session?> GetByID(int sessionId) => await SessionDLL.GetByID(sessionId);
        // ================ Read By ================
    }
}
