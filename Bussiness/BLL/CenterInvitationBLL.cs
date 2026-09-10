using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterInvitationBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private CenterInvitation _inv;
        public int _invID = -1;

        public CenterInvitationBLL()
        {
            _inv = new CenterInvitation();
            _mode = enMode.AddMode;
        }

        public CenterInvitationBLL(CenterInvitation inv)
        {
            _inv = inv;
            _mode = enMode.UpdateMode;
        }

        public CenterInvitation CurrentInvitation { get => _inv; set => _inv = value; }

        public bool Add()
        {
            _invID = CenterInvitationDLL.Add(_inv);
            return _invID > 0;
        }

        public bool Update() => CenterInvitationDLL.Update(_inv);

        public bool Delete(int id) => CenterInvitationDLL.Delete(id);

        public static Task<CenterInvitation?> GetByID(int id) => CenterInvitationDLL.GetByID(id);

        public static Task<List<CenterInvitation>> GetAll() => CenterInvitationDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
