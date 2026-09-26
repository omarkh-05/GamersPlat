using Data;
using Domain.DTOs.Center;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface ICenterService
    {
        Task<bool> Add(DTO_CreateCenter addCenter);
        Task<bool> Update(DTO_UpdateCenter center, int ownerId);
        Task<bool> UpdateActiveStatus(int centerId, int ownerId);
        Task<bool> ToggleCenterStatus(int centerId, string status, int ownerId);
        Task<bool> Delete(int centerId);
        Task<List<DTO_HomePageCentersDetails>> GetAll();
        Task<Center?> GetByID(int centerId);
        Task<DTO_CenterDetails?> GetCenterDetailsById(int centerId);
        Task<List<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId);
        Task<List<string>> GetCenterNames();
    }
}
