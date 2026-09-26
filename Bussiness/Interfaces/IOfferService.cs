using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IOfferService
    {
        Task<bool> Add(Offer offer);
        Task<bool> Update(Offer offer);
        Task<bool> Delete(int offerId);
        Task<List<Offer>> GetAll();
        Task<Offer?> GetByID(int offerId);
        Task<List<Offer>> GetByCenterId(int centerId);
    }
}
