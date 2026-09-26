using Data;
using DataLayer;
using Bussiness.Interfaces;

namespace Bussiness
{
    public class ReviewBLL : IReviewService
    {
        public int _reviewID { get; private set; }
        public int LastId => _reviewID;

        // ================ Crud ================
        public async Task<bool> Add(Review review)
        {
            int reviewID = await ReviewDLL.Add(review);
            _reviewID = reviewID;
            return reviewID > 0;
        }

        public async Task<bool> Update(Review review) => await ReviewDLL.Update(review);

        public async Task<bool> Delete(int reviewId) => await ReviewDLL.Delete(reviewId);

        public async Task<List<Review>> GetAll() => await ReviewDLL.GetAll();

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

