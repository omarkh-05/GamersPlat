using Data;
using DataLayer;
using Domain.DTOs.Player;
namespace Bussiness
{
    public class TournamentPlayerBLL
    {
        // ================ CRUD ================
        public bool Add(TournamentPlayer tp)
        {
            int _tpID = TournamentPlayerDLL.Add(tp);
            return _tpID > 0;
        }
        public async Task<List<DTO_PlayerTournamentInfo>?> GetByPlayerID(int userId) => await TournamentPlayerDLL.GetByPlayerId(userId);
        public bool DeleteByTournamentAndUser(int tournamentId, int userId) => TournamentPlayerDLL.DeleteByTournamentAndUser(tournamentId, userId);
        // ================ CRUD ================


        // ================ Read By ================
        public Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId) => TournamentPlayerDLL.GetByTournamentId(tournamentId);
        public Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId) => TournamentPlayerDLL.GetByTournamentAndUser(tournamentId, userId);
        // ================ Read By ================

    }
}
