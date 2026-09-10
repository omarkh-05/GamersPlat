using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class ReviewDLL
    {
        public static int Add(Review review)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Reviews.Add(review);
                db.SaveChanges();
                return review.ReviewId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Review Error", ex);
                return 0;
            }
        }

        public static bool Update(Review review)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Reviews.FirstOrDefault(r => r.ReviewId == review.ReviewId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(review);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Review Error", ex);
                return false;
            }
        }

        public static bool Delete(int reviewId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
                if (existing == null) return false;
                db.Reviews.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Review Error", ex);
                return false;
            }
        }

        public static async Task<Review?> GetByID(int reviewId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Center)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Review By ID Error", ex);
                return null;
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
                WriteEventLog("Get All Reviews Error", ex);
                return new List<Review>();
            }
        }

        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;
            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
