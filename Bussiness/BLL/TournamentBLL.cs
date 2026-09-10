using Data;
using DataLayer;
namespace Bussiness
{
    public class TournamentBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Tournament _t;
        public int _tID = -1;

        public TournamentBLL()
        {
            _t = new Tournament();
            _mode = enMode.AddMode;
        }

        public TournamentBLL(Tournament t)
        {
            _t = t;
            _mode = enMode.UpdateMode;
        }

        public Tournament CurrentTournament { get => _t; set => _t = value; }

        public bool Add()
        {
            _tID = TournamentDLL.Add(_t);
            return _tID > 0;
        }

        public bool Update() => TournamentDLL.Update(_t);

        public bool Delete(int id) => TournamentDLL.Delete(id);

        public static Task<Tournament?> GetByID(int id) => TournamentDLL.GetByID(id);

        public static Task<List<Tournament>> GetAll() => TournamentDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
