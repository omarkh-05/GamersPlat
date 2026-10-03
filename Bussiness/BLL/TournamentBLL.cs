using Data;
using DataLayer;
using Bussiness.Interfaces;
namespace Bussiness
{
    public class TournamentBLL : ITournamentService
    {
        public int LastId => _tID;
        public int _tID { get; private set; }

        private readonly TournamentDLL _tournamentDLL;

        public TournamentBLL(TournamentDLL tournamentDLL)
        {
            _tournamentDLL = tournamentDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Tournament t)
        {
            int tID = await _tournamentDLL.Add(t);
            _tID = tID;
            return tID > 0;
        }
        public async Task<bool> Update(Tournament t) => await _tournamentDLL.Update(t);
        public async Task<bool> Delete(int id) => await _tournamentDLL.Delete(id);
        public async Task<List<Tournament>> GetAll() => await _tournamentDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Tournament?> GetByID(int id) => await _tournamentDLL.GetByID(id);
        // ================ Read By ================
    }
}
