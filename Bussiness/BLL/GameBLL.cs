using Data;
using DataLayer;
namespace Bussiness
{
    public class GameBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Game game)
        {
            int gameID =await GameDLL.Add(game);
            return gameID > 0;
        }
        public async Task<bool> Update(Game game) => await GameDLL.Update(game);
        public async Task<bool> Delete(int id) => await GameDLL.Delete(id);
        public async Task<List<Game>> GetAll() => await GameDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Game?> GetByID(int id) => await GameDLL.GetByID(id);
        // ================ Read By ================
    }
}
