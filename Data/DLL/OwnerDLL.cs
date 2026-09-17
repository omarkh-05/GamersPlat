using Dapper;
using Data.EF;
using Domain.DTOs.Owner;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DLL
{
    public class OwnerDLL
    {
        public static async Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                using var connection = db.Database.GetDbConnection();

                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<DTO_OwnerDashboard>(
                    "dbo.sp_GetOwnerDashboard",
                    new { UserId = ownerId },
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Owner Dashboard Error", ex);
                return null;
            }
        }
    }
}
