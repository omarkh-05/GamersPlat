using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class ReviewDLL
    {
        // ================ CRUD ===========
        public static async Task<int> Add(Review review)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Reviews.Add(review);
                await db.SaveChangesAsync();
                return review.ReviewId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Review Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Review review)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Reviews.FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(review);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Review Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int reviewId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
                if (existing == null) return false;
                db.Reviews.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Review Error", ex);
                return false;
            }
        }
        public static async Task<List<Review>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Reviews
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
        public static async Task<Review?> GetByID(int reviewId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Reviews
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
        public static async Task<List<Review>?> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Reviews
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
