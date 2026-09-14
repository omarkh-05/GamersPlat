using Data;
using DataLayer;
namespace Bussiness
{
    public class ReviewBLL
    {
        // ================ Crud ================
        public bool Add(Review review)
        {
           int _reviewID = ReviewDLL.Add(review);
            return _reviewID > 0;
        }
        public bool Update(Review review) => ReviewDLL.Update(review);
        public bool Delete(int reviewId) => ReviewDLL.Delete(reviewId);
        public Task<List<Review>> GetAll() => ReviewDLL.GetAll();
        // ================ Crud ================


        // ================ Read By ================
        public Task<Review?> GetByID(int reviewId) => ReviewDLL.GetByID(reviewId);
        public async Task<List<Review>?> GetByUserId(int userId)
        {
            var reviews = await ReviewDLL.GetByUserId(userId);
            return reviews;
        }
        // ================ Read By ================
    }
}
