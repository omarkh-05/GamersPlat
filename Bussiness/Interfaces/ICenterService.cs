using Data;
using Domain.DTOs.Center;
using Domain.DTOs.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface ICenterService
    {
        Task<bool> Add(DTO_CreateCenter addCenter, int userId);
        Task<bool> Update(DTO_UpdateCenter center,int centerId, int ownerId);
        Task<bool> ActiveCenter(int centerId);
        Task<bool> DeactiveCenter(int centerId);
        Task<bool> ToggleCenterStatus(int centerId, string status, int ownerId);
        Task<bool> Delete(int centerId);
        Task<List<DTO_HomePageCentersDetails>> GetAll();
        Task<PaginatedResponse<DTO_HomePageCentersDetails>> GetAll(PaginationParameters pagination);
        Task<PaginatedResponse<DTO_HomePageCentersDetails>> GetAllForAdmin(PaginationParameters pagination);
        Task<Center?> GetByID(int centerId);
        Task<DTO_CenterDetails?> GetCenterDetailsById(int centerId);
        Task<DTO_CenterDetails?> GetCenterDetailsBySlug(string slug);
        Task<DTO_CenterOverview?> GetCenterOverView(int centerId);
        Task<List<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId);
        Task<PaginatedResponse<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId, PaginationParameters pagination);
        Task<List<string>> GetCenterNames();
    }
}
