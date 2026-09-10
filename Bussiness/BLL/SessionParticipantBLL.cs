using Data;
using DataLayer;
namespace Bussiness
{
    public class SessionParticipantBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private SessionParticipant _sp;
        public int _spID = -1;

        public SessionParticipantBLL()
        {
            _sp = new SessionParticipant();
            _mode = enMode.AddMode;
        }

        public SessionParticipantBLL(SessionParticipant sp)
        {
            _sp = sp;
            _mode = enMode.AddMode;
        }

        public SessionParticipant CurrentSessionParticipant { get => _sp; set => _sp = value; }

        public bool Add()
        {
            _spID = SessionParticipantDLL.Add(_sp);
            return _spID > 0;
        }

        public bool Delete(int id) => SessionParticipantDLL.Delete(id);

        public static Task<SessionParticipant?> GetBySessionId(int sessionId) => SessionParticipantDLL.GetBySessionId(sessionId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
