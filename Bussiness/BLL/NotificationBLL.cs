using Data;
using DataLayer;
namespace Bussiness
{
    public class NotificationBLL
    {
        // ================ CRUD ================
        public bool Add(Notification notification)
        {
            int notificationID = NotificationDLL.Add(notification);
            return notificationID > 0;
        }
        public bool Update(Notification notification) => NotificationDLL.Update(notification);
        public bool Delete(int id) => NotificationDLL.Delete(id);
        public static Task<List<Notification>> GetAll() => NotificationDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Notification?> GetByID(int id) => NotificationDLL.GetByID(id);
        // ================ Read By ================

    }
}
