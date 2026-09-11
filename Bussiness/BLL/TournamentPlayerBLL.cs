using Data;
using DataLayer;
namespace Bussiness
{
    public class TournamentPlayerBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private TournamentPlayer _tp;
        public int _tpID = -1;

        public TournamentPlayerBLL()
        {
            _tp = new TournamentPlayer();
            _mode = enMode.AddMode;
        }

        public TournamentPlayerBLL(TournamentPlayer tp)
        {
            _tp = tp;
            _mode = enMode.AddMode;
        }

        public TournamentPlayer CurrentTournamentPlayer { get => _tp; set => _tp = value; }

        public bool Add()
        {
            _tpID = TournamentPlayerDLL.Add(_tp);
            return _tpID > 0;
        }

        public static Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId) => TournamentPlayerDLL.GetByTournamentId(tournamentId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };

        public static Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId) => TournamentPlayerDLL.GetByTournamentAndUser(tournamentId, userId);

        public static bool DeleteByTournamentAndUser(int tournamentId, int userId) => TournamentPlayerDLL.DeleteByTournamentAndUser(tournamentId, userId);
    }
}
