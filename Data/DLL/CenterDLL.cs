using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using Data.DLL;
using Domain.DTOs.Center;
using Domain.DTOs.Tournuments;
using Domain.DTOs.Session;
using Domain.DTOs.Offer;
using Domain.DTOs.Resource;

namespace DataLayer
{
    public class CenterDLL
    {
        private readonly GamersPlatDbContext _db;

        public CenterDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(Center center)
        {
            try
            {
                _db.Centers.Add(center);
                await _db.SaveChangesAsync();
                return center.CenterId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Center Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Center center, int ownerId)
        {
            try
            {
                var existing = await _db.Centers
                    .FirstOrDefaultAsync(c =>
                        c.CenterId == center.CenterId &&
                        c.OwnerUserId == ownerId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(center);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Center Error", ex);
                return false;
            }
        }

        public async Task<bool> UpdateActiveStatus(int centerId, int ownerId)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c =>
                        c.CenterId == centerId &&
                        c.OwnerUserId == ownerId);

                if (center == null)
                    return false;

                center.IsActive = !center.IsActive;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Update Center Active Status Error", ex);

                return false;
            }
        }

        public async Task<bool> Delete(int centerId)
        {
            try
            {
                var existing = await _db.Centers
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);

                if (existing == null)
                    return false;

                _db.Centers.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Center Error", ex);
                return false;
            }
        }

        public async Task<List<DTO_HomePageCentersDetails>> GetAll()
        {
            try
            {
                return await _db.Centers
                    .Select(c => new DTO_HomePageCentersDetails
                    {
                        CenterName = c.CenterName,
                        CenterAddress = c.CenterAddress,
                        CenterStatus = c.CenterStatus,
                        CenterType = c.CenterType,
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        CityName = c.City.Name,
                        Rating = c.Reviews
                            .Select(r => (decimal?)r.Rating)
                            .Average() ?? 0,
                    })
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Centers Error", ex);
                return new List<DTO_HomePageCentersDetails>();
            }
        }

        public async Task<List<string>> GetCenterNames()
        {
            try
            {
                return await _db.Centers
                    .Where(c => c.IsActive == true)
                    .Select(c => c.CenterName)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center Names Error", ex);
                return new List<string>();
            }
        }

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Center?> GetByID(int centerId)
        {
            try
            {
                return await _db.Centers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.CenterId == centerId &&
                        c.IsActive == true);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center By ID Error", ex);
                return null;
            }
        }

        public async Task<DTO_CenterDetails?> GetCenterDetailsById(int centerId)
        {
            try
            {
                return await _db.Centers
                    .AsNoTracking()
                    .Where(c =>
                        c.CenterId == centerId &&
                        c.IsActive == true)
                    .Select(c => new DTO_CenterDetails
                    {
                        CenterId = c.CenterId,
                        Name = c.CenterName,
                        City = c.City.Name,
                        Country = c.City.Country.Name,
                        Description = c.CenterDescription,

                        Rating = c.Reviews
                            .Average(r => (double?)r.Rating) ?? 0,

                        ReviewCount = c.Reviews.Count(),

                        Images = c.CenterImage
                            .Select(i => i.ImageUrl)
                            .ToList(),

                        Services = c.Services
                            .Where(s => s.IsActive)
                            .Select(s => s.Name)
                            .ToList(),

                        Tournaments = c.Tournaments
                            .Where(t => t.EndDate >= DateTime.UtcNow)
                            .Select(t => new DTO_TournamentsInCenterDetails
                            {
                                TournamentId = t.TournamentId,
                                TournamentName = t.TournamentName,
                                SlotsTaken = t.TournamentPlayers.Count(),
                                MaxPlayers = t.MaxPlayers
                            })
                            .ToList(),

                        Sessions = c.Sessions
                            .Where(s => s.SessionStatus == "Open")
                            .Select(s => new DTO_SessionInCenterDetails
                            {
                                SessionId = s.SessionId,
                                Joined = s.SessionParticipants.Count(),
                                MaxPlayers = s.MaxPlayers,
                                SessionDate = s.SessionDate,
                                StartTime = s.StartTime
                            })
                            .ToList(),

                        Offers = c.Offers
                            .Where(o => o.IsActive)
                            .Select(o => new DTO_OfferInCenterDetails
                            {
                                OfferId = o.OfferId,
                                Title = o.Title,
                                Description = o.Description,
                                DiscountType = o.DiscountType ?? "No Discount",
                                DiscountValue = o.DiscountValue,
                                StartDate = o.StartDate,
                                EndDate = o.EndDate
                            })
                            .ToList(),

                        VipRooms = c.ResourcesTypes
                            .Where(r =>
                                r.RoomType == "VIP" &&
                                r.IsActive)
                            .Select(r => new DTO_ResourceType
                            {
                                ResourcesTypeId = r.ResourcesTypeId,
                                DeviceName = r.Device.DeviceName,
                                HourlyPrice = r.HourlyPrice,
                                AvailableQuantity = r.TotalQuantity
                            })
                            .ToList(),

                        NormalRooms = c.ResourcesTypes
                            .Where(r =>
                                r.RoomType == "Normal" &&
                                r.IsActive)
                            .Select(r => new DTO_ResourceType
                            {
                                ResourcesTypeId = r.ResourcesTypeId,
                                DeviceName = r.Device.DeviceName,
                                HourlyPrice = r.HourlyPrice,
                                AvailableQuantity = r.TotalQuantity
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center By ID Error", ex);
                return null;
            }
        }

        public async Task<List<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId)
        {
            try
            {
                return await _db.Centers
                    .Where(c => c.OwnerUserId == ownerId)
                    .Select(c => new DTO_CentersListDetails
                    {
                        CenterName = c.CenterName,
                        CenterAddress = c.CenterAddress,
                        CenterDescription = c.CenterDescription,
                        CenterStatus = c.CenterStatus,
                        CenterType = c.CenterType,
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        CityName = c.City.Name,

                        Rating = c.Reviews
                            .Select(r => (decimal?)r.Rating)
                            .Average() ?? 0,

                        ResourcesCount = c.ResourcesTypes
                            .Count(r => r.IsActive),

                        ServicesList = c.Services
                            .Where(s => s.IsActive)
                            .Select(s => s.Name)
                            .ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get All Centers By Owner ID Error", ex);

                return new List<DTO_CentersListDetails>();
            }
        }

        // ================ Read By ================


        // ================ Other Methods ================
        public async Task<bool> ToggleCenterStatus(
            int centerId,
            string status,
            int ownerId)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c =>
                        c.CenterId == centerId &&
                        c.OwnerUserId == ownerId);

                if (center == null)
                    return false;

                center.CenterStatus = status;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Toggle Center Status Error", ex);

                return false;
            }
        }

        // ================ Other Methods ================
    }
}