using Data;
using DataLayer;
namespace Bussiness
{
    public class GameBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Game _game;
        public int _gameID = -1;

        public GameBLL()
        {
            _game = new Game();
            _mode = enMode.AddMode;
        }

        public GameBLL(Game game)
        {
            _game = game;
            _mode = enMode.UpdateMode;
        }

        public Game CurrentGame { get => _game; set => _game = value; }

        public bool Add()
        {
            _gameID = GameDLL.Add(_game);
            return _gameID > 0;
        }

        public bool Update() => GameDLL.Update(_game);

        public bool Delete(int id) => GameDLL.Delete(id);

        public static Task<Game?> GetByID(int id) => GameDLL.GetByID(id);

        public static Task<List<Game>> GetAll() => GameDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
