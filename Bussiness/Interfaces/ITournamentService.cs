using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface ITournamentService
    {
        Task<bool> Add(Tournament t);
        Task<bool> Update(Tournament t);
        Task<bool> Delete(int id);
        Task<List<Tournament>> GetAll();
        Task<Tournament?> GetByID(int id);
        int LastId { get; }
    }
}
