using Data;
using DataLayer;
namespace Bussiness
{
    public class TournamentBLL
    {
        public int _tID { get; private set; }
        // ================ CRUD ================
        public async Task<bool> Add(Tournament t)
        {
            int tID = await TournamentDLL.Add(t);
            _tID = tID;
            return tID > 0;
        }
        public async Task<bool> Update(Tournament t) => await TournamentDLL.Update(t);
        public async Task<bool> Delete(int id) => await TournamentDLL.Delete(id);
        public async Task<List<Tournament>> GetAll() => await TournamentDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Tournament?> GetByID(int id) => await TournamentDLL.GetByID(id);
        // ================ Read By ================
    }
}
