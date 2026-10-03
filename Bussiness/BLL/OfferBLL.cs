using Data;
using DataLayer;
using Bussiness.Interfaces;

namespace Bussiness
{
    public class OfferBLL : IOfferService
    {
        private readonly OfferDLL _offerDLL;

        public OfferBLL(OfferDLL offerDLL)
        {
            _offerDLL = offerDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Offer offer)
        {
            int offerID = await _offerDLL.Add(offer);
            return offerID > 0;
        }

        public async Task<bool> Update(Offer offer)
            => await _offerDLL.Update(offer);

        public async Task<bool> Delete(int offerId)
            => await _offerDLL.Delete(offerId);

        public async Task<List<Offer>> GetAll()
            => await _offerDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Offer?> GetByID(int offerId)
            => await _offerDLL.GetByID(offerId);

        public async Task<List<Offer>> GetByCenterId(int centerId)
            => await _offerDLL.GetByCenterId(centerId);

        //public async Task<List<Offer>> GetActiveOffers()
        //    => await _offerDLL.GetActiveOffers();

        // ================ Read By ================
    }
}