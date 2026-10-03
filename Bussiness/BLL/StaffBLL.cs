using Data.DLL;
using Domain.DTOs.Staff;

namespace Bussiness.BLL
{
    public class StaffBLL
    {
        private readonly StaffDLL _staffDLL;

        public StaffBLL(StaffDLL staffDLL)
        {
            _staffDLL = staffDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(DTO_AddStaffMember staff)
            => await _staffDLL.Add(staff) > 0;
        public async Task<bool> Delete(int staffId)
            => await _staffDLL.Delete(staffId);
        public async Task<List<DTO_StaffDetails>> GetAll()
            => await _staffDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<DTO_StaffDetails?> GetStaffMemberById(int staffId)
            => await _staffDLL.GetByID(staffId);
        public async Task<List<DTO_StaffDetails>> GetAllStaff_ByCenterId(int centerId)
            => await _staffDLL.GetByCenterId(centerId);
        // ================ Read By ================
    }
}
