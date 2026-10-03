using Data;
using DataLayer;
using Domain.DTOs.Player;
using Bussiness.Interfaces;
namespace Bussiness
{
    public class TournamentPlayerBLL : ITournamentPlayerService
    {
        private readonly TournamentPlayerDLL _tournamentPlayerDLL;

        public TournamentPlayerBLL(TournamentPlayerDLL tournamentPlayerDLL)
        {
            _tournamentPlayerDLL = tournamentPlayerDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(TournamentPlayer tp)
        {
            int _tpID = await _tournamentPlayerDLL.Add(tp);
            return _tpID > 0;
        }
        public async Task<List<DTO_PlayerTournamentInfo>?> GetByPlayerID(int userId) => await _tournamentPlayerDLL.GetByPlayerId(userId);
        public async Task<bool> DeleteByTournamentAndUser(int tournamentId, int userId) => await _tournamentPlayerDLL.DeleteByTournamentAndUser(tournamentId, userId);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId) => await _tournamentPlayerDLL.GetByTournamentId(tournamentId);
        public async Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId) => await _tournamentPlayerDLL.GetByTournamentAndUser(tournamentId, userId);
        // ================ Read By ================

    }
}
