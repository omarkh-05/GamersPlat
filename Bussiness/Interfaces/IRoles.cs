using Data;

namespace Bussiness.Interfaces
{
    public interface IRoles
    {
        Task<Role?> GetByID(int roleId);
        Task<Role?> GetByName(string name);
    }
}