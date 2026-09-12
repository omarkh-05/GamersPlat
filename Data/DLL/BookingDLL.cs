using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class BookingDLL
    {
        // ================ CRUD ================
        public static int Add(Booking booking)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Bookings.Add(booking);
                db.SaveChanges();
                return booking.BookingId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Booking Error", ex);
                return 0;
            }
        }

        public static int GetBookedQuantity(int resourcesTypeId, DateOnly date)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return db.Bookings
                    .Where(b => b.ResourcesTypeId == resourcesTypeId && b.BookingDate == date && b.Status != "Cancelled")
                    .Sum(b => (int?)b.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Booked Quantity Error", ex);
                return 0;
            }
        }

        public static bool Update(Booking booking)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Bookings.FirstOrDefault(b => b.BookingId == booking.BookingId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(booking);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Booking Error", ex);
                return false;
            }
        }

        public static bool Delete(int bookingId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
                if (existing == null) return false;
                db.Bookings.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Booking Error", ex);
                return false;
            }
        }

        public static async Task<Booking?> GetByID(int bookingId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Bookings
                    .Include(b => b.Center)
                    .Include(b => b.Offer)
                    .Include(b => b.ResourcesType)
                    .Include(b => b.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Booking By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Booking>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Bookings
                    .Include(b => b.Center)
                    .Include(b => b.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Bookings Error", ex);
                return new List<Booking>();
            }
        }

        public static async Task<List<Booking>> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Bookings
                    .Where(b => b.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Bookings By User Error", ex);
                return new List<Booking>();
            }
        }

        // ===================== EventLog Helper =====================
        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;

            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
