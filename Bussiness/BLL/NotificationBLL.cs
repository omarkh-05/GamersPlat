using Data;
using DataLayer;
namespace Bussiness
{
    public class NotificationBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Notification notification)
        {
            int notificationID = await NotificationDLL.Add(notification);
            return notificationID > 0;
        }
        public async Task<bool> Update(Notification notification) => await NotificationDLL.Update(notification);
        public async Task<bool> Delete(int id) => await NotificationDLL.Delete(id);
        public async Task<List<Notification>> GetAll() => await NotificationDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Notification?> GetByID(int id) => await NotificationDLL.GetByID(id);
        // ================ Read By ================

    }
}
