using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class OfferDLL
    {
        private readonly GamersPlatDbContext _db;

        public OfferDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(Offer offer)
        {
            try
            {
                _db.Offers.Add(offer);
                await _db.SaveChangesAsync();
                return offer.OfferId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Offer Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Offer offer)
        {
            try
            {
                var existing = await _db.Offers
                    .FirstOrDefaultAsync(o => o.OfferId == offer.OfferId);

                if (existing == null) return false;

                _db.Entry(existing).CurrentValues.SetValues(offer);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Offer Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int offerId)
        {
            try
            {
                var existing = await _db.Offers
                    .FirstOrDefaultAsync(o => o.OfferId == offerId);

                if (existing == null) return false;

                _db.Offers.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Offer Error", ex);
                return false;
            }
        }

        public async Task<List<Offer>> GetAll()
        {
            try
            {
                return await _db.Offers
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
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Offer?> GetByID(int offerId)
        {
            try
            {
                return await _db.Offers
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

        public async Task<List<Offer>> GetByCenterId(int centerId)
        {
            try
            {
                return await _db.Offers
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

        // ================ Read By ================


        // ================ Filtering ===========
        //public static async Task<List<Offer>> GetActiveOffers()
        //{
        //    try
        //    {
        //        using var db = new GamersPlatDbContext();
        //        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        //        return await db.Offers
        //            .Where(o => o.IsActive && o.StartDate <= now && o.EndDate >= now)
        //            .AsNoTracking()
        //            .ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog_Helper.WriteEventLog("Get Active Offers Error", ex);
        //        return new List<Offer>();
        //    }
        //}
        // ================ Filtering ===========
    }
}