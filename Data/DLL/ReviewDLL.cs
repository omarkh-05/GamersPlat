using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class ReviewDLL
    {
        private readonly GamersPlatDbContext _db;

        public ReviewDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ===========

        public async Task<int> Add(Review review)
        {
            try
            {
                _db.Reviews.Add(review);
                await _db.SaveChangesAsync();
                return review.ReviewId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Review Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Review review)
        {
            try
            {
                var existing = await _db.Reviews
                    .FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(review);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Review Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int reviewId)
        {
            try
            {
                var existing = await _db.Reviews
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

                if (existing == null)
                    return false;

                _db.Reviews.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Review Error", ex);
                return false;
            }
        }

        public async Task<List<Review>> GetAll()
        {
            try
            {
                return await _db.Reviews
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Reviews Error", ex);
                return new List<Review>();
            }
        }

        // ================ CRUD ===========


        // ================ Read By ===========

        public async Task<Review?> GetByID(int reviewId)
        {
            try
            {
                return await _db.Reviews
                    .Include(r => r.User.FullName)
                    .Include(r => r.Center.CenterName)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Review By ID Error", ex);
                return null;
            }
        }

        public async Task<List<Review>?> GetByUserId(int userId)
        {
            try
            {
                return await _db.Reviews
                    .Where(r => r.UserId == userId)
                    .Include(r => r.User.FullName)
                    .Include(r => r.Center.CenterName)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Review By UserID Error", ex);
                return null;
            }
        }

        // ================ Read By ===========
    }
}