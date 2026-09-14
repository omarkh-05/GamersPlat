using Data;
using DataLayer;
namespace Bussiness
{
    public class OfferBLL
    {
        // ================ CRUD ================
        public bool Add(Offer offer)
        {
            int offerID = OfferDLL.Add(offer);
            return offerID > 0;
        }
        public bool Update(Offer offer) => OfferDLL.Update(offer);
        public bool Delete(int offerId) => OfferDLL.Delete(offerId);
        public static Task<List<Offer>> GetAll() => OfferDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Offer?> GetByID(int offerId) => OfferDLL.GetByID(offerId);
        public static Task<List<Offer>> GetByCenterId(int centerId) => OfferDLL.GetByCenterId(centerId);
        public static Task<List<Offer>> GetActiveOffers() => OfferDLL.GetActiveOffers();
        // ================ Read By ================
    }
}
