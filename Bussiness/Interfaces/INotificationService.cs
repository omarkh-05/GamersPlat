using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface INotificationService
    {
        Task<bool> Add(Notification notification);
        Task<bool> Update(Notification notification);
        Task<bool> Delete(int id);
        Task<List<Notification>> GetAll();
        Task<Notification?> GetByID(int id);
    }
}
