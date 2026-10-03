using Dapper;
using Data.EF;
using Domain.DTOs.Owner;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public class OwnerDLL
    {
        private readonly GamersPlatDbContext _db;

        public OwnerDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        public async Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId)
        {
            try
            {
                using var connection = _db.Database.GetDbConnection();

                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<DTO_OwnerDashboard>(
                    "dbo.sp_GetOwnerDashboard",
                    new { UserId = ownerId },
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Owner Dashboard Error",
                    ex);

                return null;
            }
        }
    }
}