using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Session _session;
        public int _sessionID = -1;

        public SessionBLL()
        {
            _session = new Session();
            _mode = enMode.AddMode;
        }

        public SessionBLL(Session session)
        {
            _session = session;
            _mode = enMode.UpdateMode;
        }

        public Session CurrentSession { get => _session; set => _session = value; }

        public bool Add()
        {
            _sessionID = SessionDLL.Add(_session);
            return _sessionID > 0;
        }

        public bool Update() => SessionDLL.Update(_session);

        public bool Delete(int sessionId) => SessionDLL.Delete(sessionId);

        public static Task<Session?> GetByID(int sessionId) => SessionDLL.GetByID(sessionId);

        public static Task<List<Session>> GetAll() => SessionDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
