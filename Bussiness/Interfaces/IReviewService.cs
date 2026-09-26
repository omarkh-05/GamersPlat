using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IReviewService
    {
        Task<bool> Add(Review review);
        Task<bool> Update(Review review);
        Task<bool> Delete(int reviewId);
        Task<List<Review>> GetAll();
        Task<Review?> GetByID(int reviewId);
        Task<List<Review>?> GetByUserId(int userId);
        int LastId { get; }
    }
}
