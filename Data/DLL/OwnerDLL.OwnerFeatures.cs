using Data.EF;
using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Owner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.DLL;

public partial class OwnerDLL
{
    public Task<IDbContextTransaction> BeginTransactionAsync() => _db.Database.BeginTransactionAsync();

    public async Task<bool> ToggleOwnerCenterActive(int centerId, int ownerId)
    {
        var center = await _db.Centers.FirstOrDefaultAsync(item => item.CenterId == centerId && item.OwnerUserId == ownerId);
        if (center == null)
            return false;
        center.IsActive = !(center.IsActive ?? false);
        center.CenterStatus = center.IsActive == true ? "Open" : "Closed";
        center.UpdatedAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<List<DTO_CentersListDetails>> GetOwnerCenters(int ownerId)
    {
        return await _db.Centers
            .AsNoTracking()
            .Where(center => center.OwnerUserId == ownerId)
            .OrderBy(center => center.CenterName)
            .Select(center => new DTO_CentersListDetails
            {
                CenterId = center.CenterId,
                CenterName = center.CenterName,
                CityName = center.City.Name,
                CountryName = center.City.Country.Name,
                CenterAddress = center.CenterAddress ?? string.Empty,
                PhoneNumber = center.PhoneNumber,
                Latitude = center.Latitude,
                Longitude = center.Longitude,
                CenterDescription = center.CenterDescription ?? string.Empty,
                CenterStatus = center.CenterStatus ?? string.Empty,
                CenterType = center.CenterType ?? string.Empty,
                IsActive = center.IsActive ?? false,
                MainImage = center.CenterImage.Where(image => image.IsMain).Select(image => image.ImageUrl).FirstOrDefault() ?? string.Empty,
                OpenTime = center.OpenTime,
                CloseTime = center.CloseTime,
                Rating = center.Reviews.Select(review => (decimal?)review.Rating).Average() ?? 0m,
                ResourcesCount = center.ResourcesTypes.Count(),
                ServicesList = center.Services.Where(service => service.IsActive).Select(service => service.Name).ToList()
            })
            .ToListAsync();
    }

    public async Task<DTO_CenterDetails?> GetOwnerCenter(int centerId, int ownerId)
    {
        return await _db.Centers
            .AsNoTracking()
            .Where(center => center.CenterId == centerId && center.OwnerUserId == ownerId)
            .Select(center => new DTO_CenterDetails
            {
                CenterId = center.CenterId,
                Name = center.CenterName,
                City = center.City.Name,
                Country = center.City.Country.Name,
                Address = center.CenterAddress ?? string.Empty,
                PhoneNumber = center.PhoneNumber,
                Latitude = center.Latitude,
                Longitude = center.Longitude,
                Description = center.CenterDescription ?? string.Empty,
                Type = center.CenterType ?? string.Empty,
                Status = center.CenterStatus ?? string.Empty,
                OpenTime = center.OpenTime,
                CloseTime = center.CloseTime,
                Rating = center.Reviews.Select(review => (decimal?)review.Rating).Average() ?? 0m,
                ReviewCount = center.Reviews.Count(),
                MainImage = center.CenterImage.Where(image => image.IsMain).Select(image => image.ImageUrl).FirstOrDefault(),
                GalleryImages = center.CenterImage.Where(image => !image.IsMain).Select(image => image.ImageUrl).ToList(),
                Services = center.Services.Where(service => service.IsActive).Select(service => service.Name).ToList(),
                Vip = center.ResourcesTypes.Where(resource => resource.IsActive && resource.RoomType != null && resource.RoomType.ToLower() == "vip")
                    .Select(resource => new Domain.DTOs.Resource.DTO_ResourceType
                    {
                        ResourcesTypeId = resource.ResourcesTypeId,
                        DeviceName = resource.Device.DeviceName,
                        Category = resource.Device.DeviceName,
                        AvailableQuantity = resource.TotalQuantity,
                        HourlyPrice = resource.HourlyPrice,
                        Description = resource.Description,
                        Unit = resource.Unit,
                        ImageUrl = resource.ImageUrl,
                        IsActive = resource.IsActive
                    }).ToList(),
                Normal = center.ResourcesTypes.Where(resource => resource.IsActive && (resource.RoomType == null || resource.RoomType.ToLower() != "vip"))
                    .Select(resource => new Domain.DTOs.Resource.DTO_ResourceType
                    {
                        ResourcesTypeId = resource.ResourcesTypeId,
                        DeviceName = resource.Device.DeviceName,
                        Category = resource.Device.DeviceName,
                        AvailableQuantity = resource.TotalQuantity,
                        HourlyPrice = resource.HourlyPrice,
                        Description = resource.Description,
                        Unit = resource.Unit,
                        ImageUrl = resource.ImageUrl,
                        IsActive = resource.IsActive
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DTO_OwnerService> AddOwnerService(int centerId, int ownerId, string name, string? description, string? icon, string? imageUrl, bool isActive)
    {
        if (!await _db.Centers.AnyAsync(center => center.CenterId == centerId && center.OwnerUserId == ownerId))
            throw new KeyNotFoundException("Center not found.");

        var service = new Service
        {
            CenterId = centerId,
            Name = name.Trim(),
            Description = description,
            Icon = icon,
            ImageUrl = imageUrl,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
        _db.Services.Add(service);
        await _db.SaveChangesAsync();

        return new DTO_OwnerService
        {
            ServiceId = service.ServiceId,
            CenterId = service.CenterId,
            Name = service.Name,
            Description = service.Description,
            Icon = service.Icon,
            ImageUrl = service.ImageUrl,
            IsActive = service.IsActive
        };
    }

    public async Task<bool> ArchiveOwnerCenter(int centerId, int ownerId)
    {
        var center = await _db.Centers.FirstOrDefaultAsync(item => item.CenterId == centerId && item.OwnerUserId == ownerId);
        if (center == null)
            return false;

        center.IsActive = false;
        center.CenterStatus = "Archived";
        center.UpdatedAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<DTO_OwnerWorkspace?> GetOwnerWorkspace(int centerId, int ownerId)
    {
        var center = await GetOwnerCenter(centerId, ownerId);
        if (center == null)
            return null;

        var resourceTotals = await _db.ResourcesTypes
            .Where(resource => resource.CenterId == centerId)
            .Select(resource => new { resource.TotalQuantity, UnitCount = resource.ResourceUnits.Count() })
            .ToListAsync();
        var unitStatuses = await _db.ResourceUnits
            .Where(unit => unit.ResourcesType.CenterId == centerId)
            .GroupBy(unit => unit.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionaryAsync(group => group.Status, group => group.Count);
        var bookingStats = await _db.Bookings
            .Where(booking => booking.CenterId == centerId)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Count = group.Count(),
                Revenue = group.Where(booking => booking.Status.ToLower() == "completed").Sum(booking => (decimal?)booking.TotalPrice) ?? 0m
            })
            .FirstOrDefaultAsync();

        return new DTO_OwnerWorkspace
        {
            Center = center,
            ResourceTypeCount = resourceTotals.Count,
            TotalUnits = resourceTotals.Sum(resource => resource.TotalQuantity),
            AvailableUnits = unitStatuses.GetValueOrDefault("Available") + resourceTotals.Where(resource => resource.UnitCount == 0).Sum(resource => resource.TotalQuantity),
            ReservedUnits = unitStatuses.GetValueOrDefault("Reserved"),
            MaintenanceUnits = unitStatuses.GetValueOrDefault("Maintenance"),
            DisabledUnits = unitStatuses.GetValueOrDefault("Disabled"),
            BookingCount = bookingStats?.Count ?? 0,
            Revenue = bookingStats?.Revenue ?? 0m,
            EmployeeCount = await _db.Staff.CountAsync(staff => staff.CenterId == centerId && staff.IsActive),
            OfferCount = await _db.Offers.CountAsync(offer => offer.CenterId == centerId && offer.IsActive),
            TournamentCount = await _db.Tournaments.CountAsync(tournament => tournament.CenterId == centerId)
        };
    }

    public async Task<List<DTO_OwnerResource>> GetOwnerResources(int centerId, int ownerId)
    {
        if (!await _db.Centers.AnyAsync(center => center.CenterId == centerId && center.OwnerUserId == ownerId))
            return [];

        var resources = await _db.ResourcesTypes
            .AsNoTracking()
            .Where(resource => resource.CenterId == centerId)
            .OrderBy(resource => resource.ResourcesTypeId)
            .Select(resource => new DTO_OwnerResource
            {
                ResourcesTypeId = resource.ResourcesTypeId,
                CenterId = resource.CenterId,
                DeviceId = resource.DeviceId,
                DeviceName = resource.Device.DeviceName,
                Category = resource.Device.DeviceName,
                RoomType = resource.RoomType,
                Description = resource.Description,
                Unit = resource.Unit,
                ImageUrl = resource.ImageUrl,
                Status = resource.IsActive ? "Active" : "Inactive",
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                IsActive = resource.IsActive,
                Units = resource.ResourceUnits.OrderBy(unit => unit.ResourceUnitId)
                    .Select(unit => new DTO_OwnerResourceUnit
                    {
                        ResourceUnitId = unit.ResourceUnitId,
                        Label = unit.Label,
                        Status = unit.Status
                    }).ToList()
            })
            .ToListAsync();

        foreach (var resource in resources.Where(resource => resource.Units.Count == 0))
        {
            resource.Units = Enumerable.Range(1, resource.TotalQuantity)
                .Select(index => new DTO_OwnerResourceUnit { Label = $"Unit {index}" })
                .ToList();
        }

        return resources;
    }

    public async Task<DTO_OwnerResource?> GetOwnerResource(int centerId, int resourceId, int ownerId)
    {
        return (await GetOwnerResources(centerId, ownerId)).FirstOrDefault(resource => resource.ResourcesTypeId == resourceId);
    }

    public async Task<bool> UpdateOwnerResourceUnits(int centerId, int resourceId, int ownerId, IReadOnlyList<DTO_OwnerResourceUnitUpdate> requestedUnits)
    {
        var resource = await _db.ResourcesTypes
            .Include(item => item.ResourceUnits)
            .FirstOrDefaultAsync(item => item.ResourcesTypeId == resourceId && item.CenterId == centerId && item.Center.OwnerUserId == ownerId);
        if (resource == null)
            return false;

        var requestIds = requestedUnits.Where(unit => unit.ResourceUnitId.HasValue && unit.ResourceUnitId.Value > 0)
            .Select(unit => unit.ResourceUnitId!.Value).ToHashSet();
        if (resource.ResourceUnits.Any(unit => unit.Status == "Reserved" && !requestIds.Contains(unit.ResourceUnitId)))
            throw new InvalidOperationException("Reserved resource units cannot be removed.");

        await using var transaction = await _db.Database.BeginTransactionAsync();
        foreach (var unitRequest in requestedUnits)
        {
            if (unitRequest.ResourceUnitId.GetValueOrDefault() > 0)
            {
                var existing = resource.ResourceUnits.FirstOrDefault(unit => unit.ResourceUnitId == unitRequest.ResourceUnitId);
                if (existing == null)
                    throw new ArgumentException("A resource unit does not belong to this resource.");
                existing.Label = unitRequest.Label.Trim();
                existing.Status = unitRequest.Status;
            }
            else
            {
                resource.ResourceUnits.Add(new ResourceUnit
                {
                    Label = unitRequest.Label.Trim(),
                    Status = unitRequest.Status
                });
            }
        }

        var retainedIds = requestIds;
        _db.ResourceUnits.RemoveRange(resource.ResourceUnits.Where(unit => !retainedIds.Contains(unit.ResourceUnitId) && unit.ResourceUnitId > 0 && !requestedUnits.Any(request => request.ResourceUnitId == unit.ResourceUnitId)));
        resource.TotalQuantity = requestedUnits.Count;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }

    public async Task<bool> ArchiveOwnerResource(int centerId, int resourceId, int ownerId)
    {
        var resource = await _db.ResourcesTypes
            .FirstOrDefaultAsync(item => item.ResourcesTypeId == resourceId && item.CenterId == centerId && item.Center.OwnerUserId == ownerId);
        if (resource == null)
            return false;
        resource.IsActive = false;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<List<DTO_OwnerService>> GetOwnerServices(int centerId, int ownerId)
    {
        return await _db.Services
            .AsNoTracking()
            .Where(service => service.CenterId == centerId && service.Center.OwnerUserId == ownerId)
            .OrderBy(service => service.ServiceId)
            .Select(service => new DTO_OwnerService
            {
                ServiceId = service.ServiceId,
                CenterId = service.CenterId,
                Name = service.Name,
                Description = service.Description,
                Icon = service.Icon,
                ImageUrl = service.ImageUrl,
                IsActive = service.IsActive
            })
            .ToListAsync();
    }

    public async Task<DTO_OwnerService?> GetOwnerService(int centerId, int serviceId, int ownerId)
    {
        return await _db.Services
            .AsNoTracking()
            .Where(service => service.CenterId == centerId && service.ServiceId == serviceId && service.Center.OwnerUserId == ownerId)
            .Select(service => new DTO_OwnerService
            {
                ServiceId = service.ServiceId,
                CenterId = service.CenterId,
                Name = service.Name,
                Description = service.Description,
                Icon = service.Icon,
                ImageUrl = service.ImageUrl,
                IsActive = service.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<DTO_BookingDetails>> GetOwnerBookings(int ownerId, DTO_OwnerBookingsQuery query)
    {
        var bookings = _db.Bookings.AsNoTracking().Where(booking => booking.Center.OwnerUserId == ownerId);
        if (query.CenterId.HasValue)
            bookings = bookings.Where(booking => booking.CenterId == query.CenterId.Value);
        if (query.Date.HasValue)
            bookings = bookings.Where(booking => booking.BookingDate == query.Date.Value);
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim().ToLower();
            bookings = bookings.Where(booking => booking.Status.ToLower() == status);
        }
        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var search = query.Q.Trim();
            bookings = bookings.Where(booking => booking.CustomerName.Contains(search) || booking.ResourcesType.Device.DeviceName.Contains(search));
        }

        var result = await bookings.OrderByDescending(booking => booking.BookingDate).ThenBy(booking => booking.StartTime)
            .Select(booking => new DTO_BookingDetails
            {
                BookingId = booking.BookingId,
                CenterId = booking.CenterId,
                PlayerId = booking.UserId,
                CustomerName = booking.CustomerName,
                PhoneNumber = booking.PhoneNumber,
                CenterName = booking.Center.CenterName,
                ResourcesType = booking.ResourcesType.Device.DeviceName + " - " + booking.ResourcesType.RoomType,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                Notes = booking.CancellationReason
            })
            .ToListAsync();

        foreach (var booking in result)
            booking.Duration = booking.EndTime.HasValue ? (booking.EndTime.Value.ToTimeSpan() - booking.StartTime.ToTimeSpan()).TotalMinutes + " Minutes" : "Open Time";

        return result;
    }

    public async Task<DTO_BookingDetails?> GetOwnerBooking(int bookingId, int ownerId)
    {
        var booking = (await GetOwnerBookings(ownerId, new DTO_OwnerBookingsQuery()))
            .FirstOrDefault(item => item.BookingId == bookingId);
        return booking;
    }

    public async Task<bool> UpdateOwnerBookingStatus(int bookingId, int ownerId, string status)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(item => item.BookingId == bookingId && item.Center.OwnerUserId == ownerId);
        if (booking == null)
            return false;
        booking.Status = status;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.CancelledAt = status == "cancelled" ? DateTime.UtcNow : null;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<List<DTO_OwnerCustomerRequest>> GetOwnerRequests(int ownerId, string? type, string? status)
    {
        var requests = _db.CustomerRequests.AsNoTracking().Where(request => request.Center.OwnerUserId == ownerId);
        if (!string.IsNullOrWhiteSpace(type))
        {
            var requestType = type.Trim().ToLower();
            requests = requests.Where(request => request.Type.ToLower() == requestType);
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            var requestStatus = status.Trim().ToLower();
            requests = requests.Where(request => request.Status.ToLower() == requestStatus);
        }

        return await requests.OrderByDescending(request => request.CreatedAt)
            .Select(request => new DTO_OwnerCustomerRequest
            {
                RequestId = request.RequestId,
                CenterId = request.CenterId,
                PlayerId = request.PlayerId,
                Type = request.Type,
                Player = request.Player != null ? request.Player.FullName : request.PlayerName,
                Subject = request.Subject,
                Message = request.Message,
                Date = request.CreatedAt,
                Status = request.Status
            })
            .ToListAsync();
    }

    public async Task<DTO_OwnerCustomerRequest?> GetOwnerRequest(int requestId, int ownerId)
    {
        return await _db.CustomerRequests
            .AsNoTracking()
            .Where(request => request.RequestId == requestId && request.Center.OwnerUserId == ownerId)
            .Select(request => new DTO_OwnerCustomerRequest
            {
                RequestId = request.RequestId,
                CenterId = request.CenterId,
                PlayerId = request.PlayerId,
                Type = request.Type,
                Player = request.Player != null ? request.Player.FullName : request.PlayerName,
                Subject = request.Subject,
                Message = request.Message,
                Date = request.CreatedAt,
                Status = request.Status
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateOwnerRequestStatus(int requestId, int ownerId, string status)
    {
        var request = await _db.CustomerRequests.FirstOrDefaultAsync(item => item.RequestId == requestId && item.Center.OwnerUserId == ownerId);
        if (request == null)
            return false;
        if (!request.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only pending requests can be accepted or rejected.");
        request.Status = status;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<List<DTO_OwnerEmployee>> GetOwnerEmployees(int ownerId, int? centerId)
    {
        var employees = _db.Staff.AsNoTracking().Where(staff => staff.Center.OwnerUserId == ownerId);
        if (centerId.HasValue)
            employees = employees.Where(staff => staff.CenterId == centerId.Value);

        return await employees.OrderBy(staff => staff.User.FullName)
            .Select(staff => new DTO_OwnerEmployee
            {
                StaffId = staff.StaffId,
                UserId = staff.UserId,
                CenterId = staff.CenterId,
                FullName = staff.User.FullName,
                PhoneNumber = staff.User.PhoneNumber,
                Email = staff.User.Email,
                CenterName = staff.Center.CenterName,
                Role = staff.StaffRole.Name,
                Permissions = staff.StaffRole.Permissions,
                IsActive = staff.IsActive
            })
            .ToListAsync();
    }

    public async Task<DTO_OwnerEmployee?> GetOwnerEmployee(int staffId, int ownerId)
    {
        return await _db.Staff
            .AsNoTracking()
            .Where(staff => staff.StaffId == staffId && staff.Center.OwnerUserId == ownerId)
            .Select(staff => new DTO_OwnerEmployee
            {
                StaffId = staff.StaffId,
                UserId = staff.UserId,
                CenterId = staff.CenterId,
                FullName = staff.User.FullName,
                PhoneNumber = staff.User.PhoneNumber,
                Email = staff.User.Email,
                CenterName = staff.Center.CenterName,
                Role = staff.StaffRole.Name,
                Permissions = staff.StaffRole.Permissions,
                IsActive = staff.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetOrCreateOwnerStaffRole(int centerId, int ownerId, string name, int permissions)
    {
        if (!await _db.Centers.AnyAsync(center => center.CenterId == centerId && center.OwnerUserId == ownerId))
            throw new KeyNotFoundException("Center not found.");
        var role = await _db.StaffRoles.FirstOrDefaultAsync(item => item.CenterId == centerId && item.Name == name.Trim());
        if (role != null)
        {
            role.Permissions = permissions;
            await _db.SaveChangesAsync();
            return role.StaffRoleId;
        }

        role = new StaffRole { CenterId = centerId, Name = name.Trim(), Permissions = permissions };
        _db.StaffRoles.Add(role);
        await _db.SaveChangesAsync();
        return role.StaffRoleId;
    }

    public async Task<int> AddOwnerEmployee(int userId, int centerId, int staffRoleId, int ownerId)
    {
        var isValid = await _db.Centers.AnyAsync(center => center.CenterId == centerId && center.OwnerUserId == ownerId)
            && await _db.StaffRoles.AnyAsync(role => role.StaffRoleId == staffRoleId && role.CenterId == centerId);
        if (!isValid)
            throw new UnauthorizedAccessException("The employee center or role is not owned by this owner.");

        var staff = new Staff
        {
            UserId = userId,
            CenterId = centerId,
            StaffRoleId = staffRoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Staff.Add(staff);
        await _db.SaveChangesAsync();
        return staff.StaffId;
    }

    public async Task<bool> UpdateOwnerEmployee(int staffId, int ownerId, int centerId, int staffRoleId, string fullName, string phoneNumber, string? email, short cityId, bool isActive)
    {
        var staff = await _db.Staff.Include(item => item.User)
            .FirstOrDefaultAsync(item => item.StaffId == staffId && item.Center.OwnerUserId == ownerId);
        if (staff == null)
            return false;
        if (!await _db.Centers.AnyAsync(center => center.CenterId == centerId && center.OwnerUserId == ownerId)
            || !await _db.StaffRoles.AnyAsync(role => role.StaffRoleId == staffRoleId && role.CenterId == centerId))
            throw new UnauthorizedAccessException("The employee center or role is not owned by this owner.");

        staff.CenterId = centerId;
        staff.StaffRoleId = staffRoleId;
        staff.IsActive = isActive;
        staff.User.FullName = fullName.Trim();
        staff.User.PhoneNumber = phoneNumber.Trim();
        staff.User.Email = email;
        staff.User.CityId = cityId;
        staff.User.UpdatedAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> ArchiveOwnerEmployee(int staffId, int ownerId)
    {
        var staff = await _db.Staff.FirstOrDefaultAsync(item => item.StaffId == staffId && item.Center.OwnerUserId == ownerId);
        if (staff == null)
            return false;
        staff.IsActive = false;
        return await _db.SaveChangesAsync() > 0;
    }
}
