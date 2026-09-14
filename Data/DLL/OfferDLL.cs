using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class OfferDLL
    {
        // ================ CRUD ================
        public static int Add(Offer offer)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Offers.Add(offer);
                db.SaveChanges();
                return offer.OfferId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Offer Error", ex);
                return 0;
            }
        }

        public static bool Update(Offer offer)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Offers.FirstOrDefault(o => o.OfferId == offer.OfferId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(offer);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Offer Error", ex);
                return false;
            }
        }

        public static bool Delete(int offerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Offers.FirstOrDefault(o => o.OfferId == offerId);
                if (existing == null) return false;
                db.Offers.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Offer Error", ex);
                return false;
            }
        }

        public static async Task<Offer?> GetByID(int offerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Offers
                    .Include(o => o.Center)
                    .Include(o => o.Bookings)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OfferId == offerId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Offer By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Offer>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Offers
                    .Include(o => o.Center)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Offers Error", ex);
                return new List<Offer>();
            }
        }

        // ================ Relations ===========
        public static async Task<List<Offer>> GetByCenterId(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Offers
                    .Where(o => o.CenterId == centerId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Offers By Center Error", ex);
                return new List<Offer>();
            }
        }

        // ================ Filtering ===========
        public static async Task<List<Offer>> GetActiveOffers()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var now = DateTime.UtcNow;
                return await db.Offers
                    .Where(o => o.IsActive && o.StartDate <= now && o.EndDate >= now)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Active Offers Error", ex);
                return new List<Offer>();
            }
        }

        // ===================== EventLog Helper =====================

    }
}
