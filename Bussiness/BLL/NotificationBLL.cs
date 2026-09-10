using Data;
using DataLayer;
namespace Bussiness
{
    public class NotificationBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Notification _notification;
        public int _notificationID = -1;

        public NotificationBLL()
        {
            _notification = new Notification();
            _mode = enMode.AddMode;
        }

        public NotificationBLL(Notification n)
        {
            _notification = n;
            _mode = enMode.UpdateMode;
        }

        public Notification CurrentNotification { get => _notification; set => _notification = value; }

        public bool Add()
        {
            _notificationID = NotificationDLL.Add(_notification);
            return _notificationID > 0;
        }

        public bool Update() => NotificationDLL.Update(_notification);

        public bool Delete(int id) => NotificationDLL.Delete(id);

        public static Task<Notification?> GetByID(int id) => NotificationDLL.GetByID(id);

        public static Task<List<Notification>> GetAll() => NotificationDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
