using Data;
using DataLayer;
namespace Bussiness
{
    public class TournamentBLL
    {
        // ================ CRUD ================
        public bool Add(Tournament t)
        {
            int tID = TournamentDLL.Add(t);
            return tID > 0;
        }
        public bool Update(Tournament t) => TournamentDLL.Update(t);
        public bool Delete(int id) => TournamentDLL.Delete(id);
        public static Task<List<Tournament>> GetAll() => TournamentDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Tournament?> GetByID(int id) => TournamentDLL.GetByID(id);
        // ================ Read By ================
    }
}
