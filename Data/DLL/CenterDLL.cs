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
        public async Task<bool> Update(DTO_UpdateCenter center,int centerId, int ownerId)
        {
            try
            {
                var existing = await _db.Centers.FirstOrDefaultAsync(c => c.CenterId == centerId && c.OwnerUserId == ownerId);

                if (existing == null)
                    return false;

                existing.CenterName = center.CenterName;
                existing.CenterDescription = center.CenterDescription;
                existing.CityId = center.CityId;
                existing.CenterAddress = center.CenterAddress;
                existing.CenterPhone = center.CenterPhone;
                existing.CenterType = center.CenterType;
                existing.OpenTime = center.OpenTime;
                existing.CloseTime = center.CloseTime;
                existing.UpdatedAt = DateTime.Now;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Center Error", ex);
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
                        CenterId = c.CenterId,
                        CenterName = c.CenterName,
                        CenterAddress = c.CenterAddress??"",
                        CenterStatus = c.CenterStatus??"busy",
                        CenterType = c.CenterType??"Gaming Center",
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        CityName = c.City.Name,
                        Rating = c.Reviews
                            .Select(r => (decimal?)r.Rating)
                            .Average() ?? 0,
                        MainImage = c.CenterImages
                            .Where(i => i.IsMain)
                            .Select(i => i.ImageUrl)
                            .FirstOrDefault() ?? "",
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
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);
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
                    .Where(c => c.CenterId == centerId && c.IsActive == true)
                    .Select(c => new DTO_CenterDetails
                    {
                        CenterId = c.CenterId,
                        Name = c.CenterName,
                        City = c.City.Name,
                        Address = c.CenterAddress??"",
                        Description = c.CenterDescription??"No Description",
                        Type = c.CenterType??"Gaming Center",
                        Status = c.CenterStatus??"closed",
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,

                        MainImage = c.CenterImages
                            .Where(i => i.IsMain)
                            .Select(i => i.ImageUrl)
                            .FirstOrDefault() ?? "",
                        GalleryImages = c.CenterImages
                            .Where(i => i.IsMain == false )
                            .Select(i => i.ImageUrl)
                            .ToList(),

                        Services = c.CenterServices
                            .Where(s => s.IsActive)
                            .Select(s => s.Service.Name)
                            .ToList(),

                        Vip = c.CenterResources
                            .Where(r => r.RoomType == "vip" && r.IsActive)
                            .Select(r => new DTO_ResourceType
                            {
                                CenterResourceId = r.CenterResourceId,
                                ResourcesId = r.ResourcesTypeId,
                                DeviceName = r.ResourcesType.ResourceName,
                                Description = r.Description,
                                HourlyPrice = r.HourlyPrice,
                                AvailableQuantity = r.TotalQuantity
                            })
                            .ToList(),
                        Private = c.CenterResources
                            .Where(r => r.RoomType == "private" && r.IsActive)
                            .Select(r => new DTO_ResourceType
                            {
                                CenterResourceId = r.CenterResourceId,
                                ResourcesId = r.ResourcesTypeId,
                                DeviceName = r.ResourcesType.ResourceName,
                                Description = r.Description,
                                HourlyPrice = r.HourlyPrice,
                                AvailableQuantity = r.TotalQuantity
                            })
                            .ToList(),
                        Public = c.CenterResources
                            .Where(r => r.RoomType == "public" && r.IsActive)
                            .Select(r => new DTO_ResourceType
                            {
                                CenterResourceId = r.CenterResourceId,
                                ResourcesId = r.ResourcesTypeId,
                                DeviceName = r.ResourcesType.ResourceName,
                                Description = r.Description,
                                HourlyPrice = r.HourlyPrice,
                                AvailableQuantity = r.TotalQuantity
                            })
                            .ToList()

                        /* Rating = c.Reviews
                             .Average(r => (double?)r.Rating) ?? 0,

                         ReviewCount = c.Reviews.Count(),

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
                        */
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
                        CenterId = c.CenterId,
                        CenterName = c.CenterName,
                        CenterAddress = c.CenterAddress ?? "",
                        CenterStatus = c.CenterStatus ?? "Busy",
                        CenterType = c.CenterType ?? "Gaming Center",
                        CenterDescription = c.CenterDescription ?? "No description available",
                        CenterPhone = c.CenterPhone ?? "No phone number available",
                        CenterImage = c.CenterImages.FirstOrDefault(i => i.IsMain).ImageUrl ?? "No image available",
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        CityName = c.City.Name,

                        EmployeeCount = c.Staff.Count(e => e.IsActive),

                        ResourcesCount = c.CenterResources
                            .Count(r => r.IsActive),
                     
                        ServicesList = c.CenterServices
                            .Where(s => s.IsActive)
                            .Select(s => s.Service.Name)
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
        public async Task<DTO_CenterOverview?> GetCenterOverView(int centerId)
        {
            try
            {
                return await _db.Centers
                    .AsNoTracking()
                    .Where(c => c.CenterId == centerId)
                    .Select(c => new DTO_CenterOverview
                    {
                        CenterName = c.CenterName,
                        CityName = c.City.Name,
                        CenterStatus = c.CenterStatus,
                        CenterAddress = c.CenterAddress,

                        ImageUrl = c.CenterImages
                            .Where(i => i.IsMain == true)
                            .Select(i => i.ImageUrl)
                            .FirstOrDefault(),

                        ResourceCount = c.CenterResources.Count(),
                     
                        Units = c.CenterResources
                            .Sum(r => (int?)r.TotalQuantity) ?? 0,

                        BookingsCount = c.Bookings.Count(),

                        Revenue = c.Bookings
                        .Where(b => b.Status == "completed")
                        .Sum(b => (decimal?)b.TotalPrice) ?? 0m
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center Overview Error", ex);
                return null;
            }
        }
        // ================ Read By ================


        // ================ Other Methods ================
        public async Task<bool> ToggleCenterStatus(int centerId, string status, int ownerId)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c => c.CenterId == centerId && c.OwnerUserId == ownerId);

                if (center == null)
                    return false;

                center.CenterStatus = status;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Toggle Center Status Error", ex);

                return false;
            }
        }
        public async Task<bool> ActiveCenter(int centerId)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);

                if (center == null)
                    return false;
                center.IsActive = true;
                center.CenterStatus = "active";
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Active Center Error", ex);
                return false;
            }
        }
        public async Task<bool> DeactiveCenter(int centerId)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c =>
                        c.CenterId == centerId);
                if (center == null)
                    return false;
                center.IsActive = false;
                center.CenterStatus = "inactive";
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Deactive Center Error", ex);
                return false;
            }
        }
        // ================ Other Methods ================
    }
}