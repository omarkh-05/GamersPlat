using Data;
using DataLayer;

namespace Bussiness
{
    public class GameBLL
    {
        private readonly GameDLL _gameDLL;

        public GameBLL(GameDLL gameDLL)
        {
            _gameDLL = gameDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Game game)
        {
            int gameID = await _gameDLL.Add(game);
            return gameID > 0;
        }

        public async Task<bool> Update(Game game)
            => await _gameDLL.Update(game);

        public async Task<bool> Delete(int id)
            => await _gameDLL.Delete(id);

        public async Task<List<Game>> GetAll()
            => await _gameDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Game?> GetByID(int id)
            => await _gameDLL.GetByID(id);
        // ================ Read By ================
    }
}