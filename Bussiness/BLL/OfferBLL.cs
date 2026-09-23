using Data;
using DataLayer;
namespace Bussiness
{
    public class OfferBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Offer offer)
        {
            int offerID = await OfferDLL.Add(offer);
            return offerID > 0;
        }
        public async Task<bool> Update(Offer offer) => await OfferDLL.Update(offer);
        public async Task<bool> Delete(int offerId) => await OfferDLL.Delete(offerId);
        public async Task<List<Offer>> GetAll() => await OfferDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Offer?> GetByID(int offerId) => await OfferDLL.GetByID(offerId);
        public async Task<List<Offer>> GetByCenterId(int centerId) => await OfferDLL.GetByCenterId(centerId);
        //public async Task<List<Offer>> GetActiveOffers() => await OfferDLL.GetActiveOffers();
        // ================ Read By ================
    }
}
