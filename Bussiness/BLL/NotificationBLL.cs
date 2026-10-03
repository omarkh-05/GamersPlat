using Data;
using DataLayer;
using Bussiness.Interfaces;

namespace Bussiness
{
    public class NotificationBLL : INotificationService
    {
        private readonly NotificationDLL _notificationDLL;

        public NotificationBLL(NotificationDLL notificationDLL)
        {
            _notificationDLL = notificationDLL;
        }

        // ================ CRUD ================

        public async Task<bool> Add(Notification notification)
        {
            int notificationID = await _notificationDLL.Add(notification);
            return notificationID > 0;
        }

        public async Task<bool> Update(Notification notification)
            => await _notificationDLL.Update(notification);

        public async Task<bool> Delete(int id)
            => await _notificationDLL.Delete(id);

        public async Task<List<Notification>> GetAll()
            => await _notificationDLL.GetAll();

        // ================ CRUD ================


        // ================ Read By ================

        public async Task<Notification?> GetByID(int id)
            => await _notificationDLL.GetByID(id);

        // ================ Read By ================
    }
}