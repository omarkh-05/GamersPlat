using Data;
using DataLayer;
namespace Bussiness
{
    public class OfferBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Offer _offer;
        public int _offerID = -1;

        public OfferBLL()
        {
            _offer = new Offer();
            _mode = enMode.AddMode;
        }

        public OfferBLL(Offer offer)
        {
            _offer = offer;
            _mode = enMode.UpdateMode;
        }

        public Offer CurrentOffer { get => _offer; set => _offer = value; }

        public bool Add()
        {
            _offerID = OfferDLL.Add(_offer);
            return _offerID > 0;
        }

        public bool Update() => OfferDLL.Update(_offer);

        public bool Delete(int offerId) => OfferDLL.Delete(offerId);

        public static Task<Offer?> GetByID(int offerId) => OfferDLL.GetByID(offerId);

        public static Task<List<Offer>> GetAll() => OfferDLL.GetAll();

        public static Task<List<Offer>> GetByCenterId(int centerId) => OfferDLL.GetByCenterId(centerId);

        public static Task<List<Offer>> GetActiveOffers() => OfferDLL.GetActiveOffers();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
