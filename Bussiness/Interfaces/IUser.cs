using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.User;

namespace Bussiness.Interfaces
{
    public interface IUser
    {
        Task<bool> Add(User user);
        Task<bool> Update(int userId, DTO_UpdateUserInfoRequest dto);
        Task<bool> Delete(int userId);

        Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId);
        Task<List<DTO_UserListResponse>> GetAll();
        Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phone);
        Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass);
        Task<DTO_UserInfoRequest?> GetById(int userId);

        Task<bool> ExistsByEmail(string? email, int excludeId = 0);
        Task<bool> ExistsByPhone(string? phone, int excludeId = 0);


        string? LastError { get; }
    }
}
