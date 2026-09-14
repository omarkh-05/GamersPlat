using Data.EF;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data.DLL
{
    public class PlayerDLL
    {
        // ================ Read By ================
        public static async Task<DTO_PlayerProfile?> GetPlayerProfile(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                using var connection = db.Database.GetDbConnection();

                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<DTO_PlayerProfile>(
                    "dbo.sp_GetPlayerStatistics",
                    new { UserId = userId },
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Player Statistics Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
