using Data;
using DataLayer;
namespace Bussiness
{
    public class GameBLL
    {
        // ================ CRUD ================
        public bool Add(Game game)
        {
            int gameID = GameDLL.Add(game);
            return gameID > 0;
        }
        public bool Update(Game game) => GameDLL.Update(game);
        public bool Delete(int id) => GameDLL.Delete(id);
        public static Task<List<Game>> GetAll() => GameDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Game?> GetByID(int id) => GameDLL.GetByID(id);
        // ================ Read By ================
    }
}
