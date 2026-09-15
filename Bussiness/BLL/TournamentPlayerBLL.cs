using Data;
using DataLayer;
using Domain.DTOs.Player;
namespace Bussiness
{
    public class TournamentPlayerBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(TournamentPlayer tp)
        {
            int _tpID = await TournamentPlayerDLL.Add(tp);
            return _tpID > 0;
        }
        public async Task<List<DTO_PlayerTournamentInfo>?> GetByPlayerID(int userId) => await TournamentPlayerDLL.GetByPlayerId(userId);
        public async Task<bool> DeleteByTournamentAndUser(int tournamentId, int userId) => await TournamentPlayerDLL.DeleteByTournamentAndUser(tournamentId, userId);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId) => await TournamentPlayerDLL.GetByTournamentId(tournamentId);
        public async Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId) => await TournamentPlayerDLL.GetByTournamentAndUser(tournamentId, userId);
        // ================ Read By ================

    }
}
