using Data;
using Data.DLL;
using Domain.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bussiness.BLL
{
    public class StaffRoleBLL
    {
        // ================ Role Management ================
        public async Task<bool> AddStaffRole(DTO_StaffRole role)
        {
            if(role == null || string.IsNullOrEmpty(role.Name) || role.Permissions < 0)
            {
                return false;
            }

            StaffRole staffRole = new StaffRole
            {
                CenterId = role.CenterId,
                Name = role.Name,
                Permissions = role.Permissions
            };

            return await StaffRoleDLL.AddStaffRole(staffRole);

        }
        public async Task<bool> SetRoleToMember(DTO_SetRoleToMember request)
        {
            if (request == null || request.StaffRoleId <= 0 || request.StaffId <= 0)
            {
                return false;
            }

            var staffRole = new Staff
            {
                StaffId = request.StaffId,
                StaffRoleId = request.StaffRoleId,
            };

            return await StaffRoleDLL.SetRoleToMember(staffRole);
        }
        // ================ Role Management ================
    }
}
