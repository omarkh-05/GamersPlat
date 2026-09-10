using Data;
using DataLayer;
namespace Bussiness
{
    public class ReviewBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Review _review;
        public int _reviewID = -1;

        public ReviewBLL()
        {
            _review = new Review();
            _mode = enMode.AddMode;
        }

        public ReviewBLL(Review review)
        {
            _review = review;
            _mode = enMode.UpdateMode;
        }

        public Review CurrentReview { get => _review; set => _review = value; }

        public bool Add()
        {
            _reviewID = ReviewDLL.Add(_review);
            return _reviewID > 0;
        }

        public bool Update() => ReviewDLL.Update(_review);

        public bool Delete(int reviewId) => ReviewDLL.Delete(reviewId);

        public static Task<Review?> GetByID(int reviewId) => ReviewDLL.GetByID(reviewId);

        public static Task<List<Review>> GetAll() => ReviewDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
