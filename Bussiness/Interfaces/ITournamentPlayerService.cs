using Data;
using Domain.DTOs.Player;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface ITournamentPlayerService
    {
        Task<bool> Add(TournamentPlayer tp);
        Task<List<DTO_PlayerTournamentInfo>?> GetByPlayerID(int userId);
        Task<bool> DeleteByTournamentAndUser(int tournamentId, int userId);
        Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId);
        Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId);
    }
}
