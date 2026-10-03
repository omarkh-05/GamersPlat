using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class CenterInvitationDLL
    {
        private readonly GamersPlatDbContext _db;

        public CenterInvitationDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(CenterInvitation inv)
        {
            try
            {
                _db.CenterInvitations.Add(inv);
                await _db.SaveChangesAsync();

                return inv.InvitationId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Add CenterInvitation Error",
                    ex);

                return 0;
            }
        }

        public async Task<bool> Update(CenterInvitation inv)
        {
            try
            {
                var existing = await _db.CenterInvitations
                    .FirstOrDefaultAsync(i => i.InvitationId == inv.InvitationId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(inv);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Update CenterInvitation Error",
                    ex);

                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.CenterInvitations
                    .FirstOrDefaultAsync(i => i.InvitationId == id);

                if (existing == null)
                    return false;

                _db.CenterInvitations.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Delete CenterInvitation Error",
                    ex);

                return false;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterInvitation?> GetByID(int id)
        {
            try
            {
                return await _db.CenterInvitations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.InvitationId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get CenterInvitation By ID Error",
                    ex);

                return null;
            }
        }

        public async Task<List<CenterInvitation>> GetAll()
        {
            try
            {
                return await _db.CenterInvitations
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get All CenterInvitations Error",
                    ex);

                return new List<CenterInvitation>();
            }
        }
        // ================ Read By ================
    }
}