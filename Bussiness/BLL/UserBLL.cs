using Bussiness.Interfaces;
using Data;
using Data.DLL;
using DataLayer;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using Domain.DTOs.Pagination;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace Bussiness
{
    public class UserBLL : IUser
    {
        private readonly UserDLL _userDLL;
        readonly CityBLL _cityBLL;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string? LastError { get; private set; }
        public UserBLL(UserDLL userDLL, CityBLL cityBLL, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _userDLL = userDLL;
            _cityBLL = cityBLL;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }
        // ================ CRUD ================
        public async Task<bool> Add(User user)
        {
            var userId = await _userDLL.Add(user);
            return userId > 0;
        }
        public async Task<bool> Update(int userId, DTO_UpdateUserInfoRequest dto, int adminId=0)
        {
              LastError = null;

                if (dto == null)
                {
                    LastError = "Invalid request";
                    return false;
                }

                dto.FullName = dto.FullName?.Trim() ?? "";
                dto.PhoneNumber = dto.PhoneNumber?.Trim() ?? "";
                dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();

                if (dto.FullName.Length < 2)
                {
                    LastError = "Full name is required";
                    return false;
                }

                if (dto.PhoneNumber.Length == 0)
                {
                    LastError = "Phone number is required";
                    return false;
                }

                if (dto.CityId <= 0)
                {
                    LastError = "City is required";
                    return false;
                }

                if (!await _userDLL.ExistsById(userId))
                {
                    LastError = "User not found";
                    return false;
                }

                if (await _userDLL.ExistsByPhone(dto.PhoneNumber, userId))
                {
                    LastError = "Phone number is already in use (conflict)";
                    return false;
                }


                if(!await _cityBLL.CityExist(dto.CityId))
                {
                LastError = "City not found";
                    return false;
                }

                var oldValues = await _userDLL.GetById(userId);
                bool updated = await _userDLL.Update(userId, dto, adminId);
                if (!updated)
                {
                    LastError = "An unexpected error occurred while updating the profile";
                    return false;
                }

                var newValues = await _userDLL.GetById(userId);
                var actorId = adminId > 0 ? adminId : GetCurrentUserId() ?? userId;
                await _auditLogService.LogAsync(actorId, "UserUpdated", "User", userId, oldValues, newValues);
            return true;
        }
        public async Task<bool> Delete(int userId)
        {
            var oldValues = await _userDLL.GetById(userId);
            var deleted = await _userDLL.Delete(userId);
            if (deleted)
            {
                var actorId = GetCurrentUserId();
                if (actorId == userId)
                    actorId = null;
                await _auditLogService.LogAsync(actorId, "UserDeleted", "User", userId, oldValues, null);
            }
            return deleted;
        }
        public async Task<List<DTO_UserListResponse>> GetAll()
        {
            return await _userDLL.GetAll();
        }
        public async Task<PaginatedResponse<DTO_UserListResponse>> GetAll(PaginationParameters pagination)
        {
            return await _userDLL.GetAll(pagination);
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId)
        {
            return await _userDLL.GetUserInfoByID(userId);
        }
        public async Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phone)
        {
            return await _userDLL.GetUserInfoByPhone(phone);
        }
        public async Task<DTO_UserInfoRequest?> GetById(int userId)
        {
            return await _userDLL.GetById(userId);
        }
        public async Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass)
        {
            return await _userDLL.GetIdByPhoneOrEmail(reqResetPass);
        }
        // ================ Read By ================


        // ================ Validation ============
        public async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            return await _userDLL.ExistsByEmail(email, excludeId);
        }
        public async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            return await _userDLL.ExistsByPhone(phone, excludeId);
        }
        // ================ Validation ============

        private int? GetCurrentUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var userId) && userId > 0 ? userId : null;
        }
    }
}
