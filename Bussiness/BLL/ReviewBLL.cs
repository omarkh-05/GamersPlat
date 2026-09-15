using Data;
using DataLayer;
namespace Bussiness
{
    public class ReviewBLL
    {
        // ================ Crud ================
        public async Task<bool> Add(Review review)
        {
           int _reviewID = await ReviewDLL.Add(review);
            return _reviewID > 0;
        }
        public async Task<bool> Update(Review review) => await ReviewDLL.Update(review);
        public async Task<bool> Delete(int reviewId) => await ReviewDLL.Delete(reviewId);
        public async Task<List<Review>> GetAll() => await ReviewDLL.GetAll();
        // ================ Crud ================


        // ================ Read By ================
        public async Task<Review?> GetByID(int reviewId) => await ReviewDLL.GetByID(reviewId);
        public async Task<List<Review>?> GetByUserId(int userId)
        {
            var reviews = await ReviewDLL.GetByUserId(userId);
            return reviews;
        }
        // ================ Read By ================
    }
}
