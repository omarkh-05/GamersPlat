using Data;
using DataLayer;
namespace Bussiness
{
    public class PointTransactionBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private PointTransaction _pt;
        public int _ptID = -1;

        public PointTransactionBLL()
        {
            _pt = new PointTransaction();
            _mode = enMode.AddMode;
        }

        public PointTransactionBLL(PointTransaction pt)
        {
            _pt = pt;
            _mode = enMode.AddMode;
        }

        public PointTransaction CurrentPointTransaction { get => _pt; set => _pt = value; }

        public bool Add()
        {
            _ptID = PointTransactionDLL.Add(_pt);
            return _ptID > 0;
        }

        public static Task<List<PointTransaction>> GetByUserId(int userId) => PointTransactionDLL.GetByUserId(userId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
