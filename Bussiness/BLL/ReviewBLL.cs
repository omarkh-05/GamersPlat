using Data;
using DataLayer;
using Bussiness.Interfaces;

namespace Bussiness
{
    public class ReviewBLL : IReviewService
    {
        private readonly ReviewDLL _reviewDLL;

        public int _reviewID { get; private set; }
        public int LastId => _reviewID;

        public ReviewBLL(ReviewDLL reviewDLL)
        {
            _reviewDLL = reviewDLL;
        }

        // ================ Crud ================

        public async Task<bool> Add(Review review)
        {
            int reviewID = await _reviewDLL.Add(review);
            _reviewID = reviewID;
            return reviewID > 0;
        }

        public async Task<bool> Update(Review review)
            => await _reviewDLL.Update(review);

        public async Task<bool> Delete(int reviewId)
            => await _reviewDLL.Delete(reviewId);

        public async Task<List<Review>> GetAll()
            => await _reviewDLL.GetAll();

        // ================ Read By ================

        public async Task<Review?> GetByID(int reviewId)
            => await _reviewDLL.GetByID(reviewId);

        public async Task<List<Review>?> GetByUserId(int userId)
            => await _reviewDLL.GetByUserId(userId);

        // ================ Read By ================
    }
}