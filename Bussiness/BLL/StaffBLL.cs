using Data.DLL;
using Domain.DTOs.Staff;

namespace Bussiness.BLL
{
    public class StaffBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(DTO_AddStaffMember staff)
            => await StaffDLL.Add(staff) > 0;
        public async Task<bool> Delete(int staffId)
            => await StaffDLL.Delete(staffId);
        public async Task<List<DTO_StaffDetails>> GetAll()
            => await StaffDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<DTO_StaffDetails?> GetStaffMemberById(int staffId)
            => await StaffDLL.GetByID(staffId);
        public async Task<List<DTO_StaffDetails>> GetAllStaff_ByCenterId(int centerId)
            => await StaffDLL.GetByCenterId(centerId);
        // ================ Read By ================
    }
}
