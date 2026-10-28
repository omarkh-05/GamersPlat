using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Auth;
using Domain.DTOs.Owner;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.Staff;
using Domain.DTOs.User;

namespace Bussiness.BLL
{
    using Bussiness.Interfaces;

    public class OwnerBLL : IOwnerService
    {
        readonly IUser _user;
        readonly CenterBLL _center;
        readonly ResourcesTypeBLL _resourcesType;
        readonly ServiceBLL _services;
        readonly BookingBLL _booking;
        readonly StaffBLL _staff;
        readonly StaffRoleBLL _staffRole;
        readonly IAuthService _auth;
        private readonly OwnerDLL _ownerDLL;

        public OwnerBLL(
            IUser user,
            CenterBLL centerBLL,
            ResourcesTypeBLL resourcesTypeBLL,
            ServiceBLL servicesBLL,
            BookingBLL bookingBLL,
            StaffBLL staffBLL,
            StaffRoleBLL staffRoleBLL,
            IAuthService auth,
            OwnerDLL ownerDLL)
        {
            _user = user;
            _center = centerBLL;
            _resourcesType = resourcesTypeBLL;
            _services = servicesBLL;
            _booking = bookingBLL;
            _staff = staffBLL;
            _staffRole = staffRoleBLL;
            _auth = auth;
            _ownerDLL = ownerDLL;
        }

        // IOwnerService implementations (thin wrappers to existing methods)
        async Task<bool> IOwnerService.AcceptBooking(int bookingId, int ownerUserId) => await AcceptBooking(bookingId, ownerUserId);
        async Task<bool> IOwnerService.RejectBooking(int bookingId, int ownerUserId) => await RejectBooking(bookingId, ownerUserId);
        async Task<List<DTO_BookingDetails>> IOwnerService.GetAllBookings(int ownerUserId) => await GetAllBookings(ownerUserId);
        async Task<DTO_BookingDetails> IOwnerService.ViewBookingDetails(int bookingId, int ownerUserId) => await ViewBookingDetails(bookingId, ownerUserId);

        // ================ Owner Management ================
        public async Task<DTO_UserInfoRequest?> GetOwnerInfo(int ownerId)
        {
            if(ownerId <= 0)
            {
                throw new ArgumentException("Invalid owner ID.");
            }
            var user = await _user.GetUserInfoByID(ownerId);
            if (user == null)
            {
                throw new KeyNotFoundException("Owner not found.");
            }
            return user;
        }
        public async Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId, DateOnly? from = null, DateOnly? to = null)
        {
            if (ownerId <= 0)
            {
                throw new ArgumentException("Invalid owner ID.");
            }
            if (from.HasValue && to.HasValue && from.Value > to.Value)
                throw new ArgumentException("The start date must be on or before the end date.");

            var ownerDashboard = await _ownerDLL.GetOwnerDashboard(ownerId, from, to);
            if(ownerDashboard == null)
            {
                throw new KeyNotFoundException("Owner dashboard not found.");
            }
            return ownerDashboard;
        }
        public async Task<bool?> UpdateOwnerInfo(int ownerId, DTO_UpdateUserInfoRequest updateUserInfoRequest)
        {
            if (ownerId <= 0)
            {
                throw new ArgumentException("Invalid owner ID.");
            }
           
            bool userUpdated = await _user.Update(ownerId, updateUserInfoRequest);
            if (!userUpdated)
            {
                throw new Exception("Failed to update user information.");
            }
            return userUpdated;
        }
        // ================ Owner Management ================


        // ================ Center Management ================
        public async Task<bool> AddCenter(DTO_CreateCenter addCenter, int ownerUserId)
        {
            if(addCenter == null)
            {
                throw new ArgumentNullException(nameof(addCenter), "Center cannot be null While Creating Center");
            }
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");
            addCenter.OwnerUserId = ownerUserId;
            return await _center.Add(addCenter, ownerUserId);
        }
        public async Task<bool> UpdateCenter(DTO_UpdateCenter updateCenter, int ownerUserId)
        {
            if (updateCenter == null)
            {
                throw new ArgumentNullException(nameof(updateCenter), "Center cannot be null While Updating Center");
            }
            var center = await _ownerDLL.GetOwnerCenter(updateCenter.CenterId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            return await _center.Update(updateCenter, ownerUserId);
        }
        public async Task<bool> Active_DeactiveCenter(int centerId, int ownerUserId)
        {
            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            return await _ownerDLL.ToggleOwnerCenterActive(centerId, ownerUserId);
        }
        public async Task<bool> ToggleCenterStatus(int centerId, int ownerUserId, string status)
        {
            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if(status != "Open" && status != "Closed" && status != "Busy" && status != "Maintenance")
                throw new ArgumentException("Invalid status. Status must be either 'Open' or 'Closed' or 'Busy' or 'Maintenance'.");
            return await _center.ToggleCenterStatus(centerId, status, ownerUserId);
        }
        public async Task<List<DTO_CentersListDetails>> GetAllCentersForOwner(int ownerId)
        {
            if(ownerId <= 0)
            {
                throw new ArgumentException("Invalid owner ID.");
            }
            return await _ownerDLL.GetOwnerCenters(ownerId);
        }
        // ================ Center Management ================


        // ================ Resources Management ================
        public async Task<bool> AddResource(DTO_AddResource resource,int ownerUserId)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource), "Resource cannot be null while adding resource");
            }
            var center = await _ownerDLL.GetOwnerCenter(resource.CenterId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            var newResource = new ResourcesType
            {
                CenterId = resource.CenterId,
                DeviceId = resource.DeviceId,
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                RoomType = resource.RoomType,
                Description = resource.Description,
                Unit = resource.Unit,
                ImageUrl = resource.ImageUrl,
                IsActive = resource.IsActive
            };

            return await _resourcesType.Add(newResource);
        }
        public async Task<bool> UpdateResource(DTO_UpdateResource resource, int ownerUserId)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource), "Resource cannot be null while adding resource");
            }
            var center = await _ownerDLL.GetOwnerCenter(resource.CenterId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (await _ownerDLL.GetOwnerResource(resource.CenterId, resource.ResourcesTypeId, ownerUserId) == null)
                throw new KeyNotFoundException("Resource not found.");

            var newResource = new ResourcesType
            {
                ResourcesTypeId = resource.ResourcesTypeId,
                CenterId = resource.CenterId,
                DeviceId = resource.DeviceId,
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                RoomType = resource.RoomType,
                Description = resource.Description,
                Unit = resource.Unit,
                ImageUrl = resource.ImageUrl,
                IsActive = resource.IsActive
            };

            return await _resourcesType.Update(newResource);
        }
        public async Task<bool> ActiveResource(int resourceId,int centerId,int ownerUserId)
        {
            if (resourceId <= 0)
            {
                throw new ArgumentException("Invalid resource ID.");
            }
            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId) == null)
                throw new KeyNotFoundException("Resource not found.");

            return await _resourcesType.ActiveStatus(centerId, resourceId);
        }
        public async Task<bool> DeactiveResource(int resourceId, int centerId, int ownerUserId)
        {
            if (resourceId <= 0)
            {
                throw new ArgumentException("Invalid resource ID.");
            }
            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId) == null)
                throw new KeyNotFoundException("Resource not found.");

            return await _resourcesType.DeactiveStatus(centerId, resourceId);
        }
        // ================ Resource Management ================


        // ================ Services Management ================
        public async Task<bool> AddService(DTO_AddService service, int ownerUserId)
        {
            if (service == null)
            {
                throw new ArgumentNullException( nameof(service),"Service cannot be null while adding service");
            }

            var center = await _ownerDLL.GetOwnerCenter(service.CenterId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            var newService = new Service
            {
                CenterId = service.CenterId,
                Name = service.Name,
                Description = service.Description,
                Icon = service.Icon,
                ImageUrl = service.ImageUrl,
                IsActive = service.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            return await _services.Add(newService);
        }
        public async Task<bool> UpdateService(DTO_UpdateService service,int ownerUserId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service),"Service cannot be null while updating service");
            }

            var center = await _ownerDLL.GetOwnerCenter(service.CenterId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (await _ownerDLL.GetOwnerService(service.CenterId, service.ServiceId, ownerUserId) == null)
                throw new KeyNotFoundException("Service not found.");

            var updatedService = new Service
            {
                ServiceId = service.ServiceId,
                CenterId = service.CenterId,
                Name = service.Name,
                Description = service.Description,
                Icon = service.Icon,
                ImageUrl = service.ImageUrl,
                IsActive = service.IsActive,
                UpdatedAt = DateTime.UtcNow
            };

            return await _services.Update(updatedService);
        }
        public async Task<bool> DeleteService( int serviceId, int centerId, int ownerUserId)
        {
            if (serviceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (await _ownerDLL.GetOwnerService(centerId, serviceId, ownerUserId) == null)
                throw new KeyNotFoundException("Service not found.");

            return await _services.Delete(serviceId, centerId);
        }
        public async Task<bool> ActiveService(int serviceId,int centerId,int ownerUserId)
        {
            if (serviceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (await _ownerDLL.GetOwnerService(centerId, serviceId, ownerUserId) == null)
                throw new KeyNotFoundException("Service not found.");

            return await _services.ActiveStatus(serviceId, centerId);
        }
        public async Task<bool> DeactiveService(int serviceId, int centerId, int ownerUserId)
        {
            if (serviceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (await _ownerDLL.GetOwnerService(centerId, serviceId, ownerUserId) == null)
                throw new KeyNotFoundException("Service not found.");

            return await _services.DeactiveStatus(serviceId, centerId);
        }
        // ================ Services Management ================


        // ================ Bookings Management ================
        public async Task<bool> AddBooking(DTO_BookingBase booking, int ownerUserId)
        {
            if(booking == null)
            {
                throw new ArgumentNullException(nameof(booking), "Booking cannot be null while adding booking");
            }
            // Owner initiated bookings should be linked to the owner's user account
            return await _booking.Add(booking, ownerUserId, null, null);
        }
        public async Task<bool> UpdateBooking(DTO_UpdateBooking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking), "Booking cannot be null while updating booking");
            }
            return await _booking.Update(booking,0);
        }
        public async Task<bool> AcceptBooking(int bookingId,int ownerUserId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            var center = await _ownerDLL.GetOwnerCenter(booking.CenterId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (booking.Status != "Pending")
                throw new InvalidOperationException("Only pending bookings can be accepted.");

            return await _booking.Accept_RejectBooking( bookingId,"Accepted");
        }
        public async Task<bool> RejectBooking(int bookingId,int ownerUserId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            var center = await _ownerDLL.GetOwnerCenter(booking.CenterId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (booking.Status != "Pending")
                throw new InvalidOperationException("Only pending bookings can be rejected.");

            return await _booking.Accept_RejectBooking(bookingId,"Rejected");
        }
        public async Task<DTO_BookingDetails> ViewBookingDetails(int bookingId,int ownerUserId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            return await _ownerDLL.GetOwnerBooking(bookingId, ownerUserId)
                ?? throw new KeyNotFoundException("Booking not found.");
        }
        public async Task<List<DTO_BookingDetails>> GetAllBookings(int ownerUserId)
        {
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");

            return await _ownerDLL.GetOwnerBookings(ownerUserId, new DTO_OwnerBookingsQuery());
        }
        public async Task<List<DTO_BookingDetails>> GetAllUBookings_ByCenterId(int centerId,int ownerUserId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            var center = await _ownerDLL.GetOwnerCenter(centerId, ownerUserId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            return await _ownerDLL.GetOwnerBookings(ownerUserId, new DTO_OwnerBookingsQuery { CenterId = centerId });
        }
        // ================ Bookings Management ================

        
        // ================ Staffs Management ================
        public async Task<bool> AddStaffMember(DTO_AddStaffMember staffMember)
            => await _staff.Add(staffMember);
        public async Task<bool> UpdateStaffMember(int userId,DTO_UpdateUserInfoRequest request) 
        {
            return await _user.Update(userId, request);
        }
        public async Task<bool> DeleteStaffMember(int userId)
        {
            return await _staff.Delete(userId);
        }
        public async Task<bool> AddStaffRole_ByCenterId(DTO_StaffRole role)
        {
            return await _staffRole.AddStaffRole(role);
        }
        public async Task<bool> SetRoleToMember(DTO_SetRoleToMember request)
        {
            return await _staffRole.SetRoleToMember(request);
        }
        public async Task<List<DTO_StaffDetails>> GetAllStaff()
        {
            return await _staff.GetAll();
        }
        public async Task<List<DTO_StaffDetails>> GetAllStaff_ByCenterId(int centerId)
        {
            return await _staff.GetAllStaff_ByCenterId(centerId);
        }
        // ================ Staffs Management ================

        public async Task<DTO_CenterDetails> GetOwnerCenterDetails(int centerId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            return await _ownerDLL.GetOwnerCenter(centerId, ownerUserId)
                ?? throw new KeyNotFoundException("Center not found.");
        }

        public async Task<DTO_CenterDetails> CreateOwnerCenter(DTO_CreateCenter request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");

            request.OwnerUserId = ownerUserId;
            if (!await AddCenter(request, ownerUserId))
                throw new Exception("Failed to create center.");

            return await _ownerDLL.GetOwnerCenter(_center.LastId, ownerUserId)
                ?? throw new Exception("The created center could not be retrieved.");
        }

        public async Task<DTO_CenterDetails> UpdateOwnerCenter(int centerId, DTO_UpdateCenter request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(centerId, ownerUserId);
            request.CenterId = centerId;
            if (!await UpdateCenter(request, ownerUserId))
                throw new Exception("Failed to update center.");
            return await _ownerDLL.GetOwnerCenter(centerId, ownerUserId)
                ?? throw new KeyNotFoundException("Center not found.");
        }

        public async Task<bool> DeleteOwnerCenter(int centerId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (!await _ownerDLL.ArchiveOwnerCenter(centerId, ownerUserId))
                throw new InvalidOperationException("Failed to archive center.");
            return true;
        }

        public async Task<DTO_OwnerWorkspace> GetOwnerWorkspace(int centerId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            return await _ownerDLL.GetOwnerWorkspace(centerId, ownerUserId)
                ?? throw new KeyNotFoundException("Center workspace not found.");
        }

        public async Task<List<DTO_OwnerResource>> GetOwnerResources(int centerId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            return await _ownerDLL.GetOwnerResources(centerId, ownerUserId);
        }

        public async Task<DTO_OwnerResource> CreateOwnerResource(int centerId, DTO_AddResource request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (request.TotalQuantity is < 1 or > 500)
                throw new ArgumentException("Resource quantity must be between 1 and 500.");
            if (request.HourlyPrice < 0)
                throw new ArgumentException("Hourly price cannot be negative.");

            request.CenterId = centerId;
            if (!await AddResource(request, ownerUserId))
                throw new Exception("Failed to create resource.");

            var resourceId = _resourcesType.LastId;
            var units = Enumerable.Range(1, request.TotalQuantity)
                .Select(index => new DTO_OwnerResourceUnitUpdate { Label = $"Unit {index}", Status = "Available" })
                .ToList();
            await _ownerDLL.UpdateOwnerResourceUnits(centerId, resourceId, ownerUserId, units);
            return await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId)
                ?? throw new Exception("The created resource could not be retrieved.");
        }

        public async Task<DTO_OwnerResource> UpdateOwnerResource(int centerId, int resourceId, DTO_UpdateResource request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId) == null)
                throw new KeyNotFoundException("Resource not found.");
            if (request.TotalQuantity is < 0 or > 500)
                throw new ArgumentException("Resource quantity must be between 0 and 500.");
            if (request.HourlyPrice < 0)
                throw new ArgumentException("Hourly price cannot be negative.");
            var existingUnits = (await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId))?.Units ?? [];
            if (request.TotalQuantity < existingUnits.Count && existingUnits.Skip(request.TotalQuantity)
                .Any(unit => unit.Status.Equals("Reserved", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Reserved resource units cannot be removed.");

            request.ResourcesTypeId = resourceId;
            request.CenterId = centerId;
            if (!await UpdateResource(request, ownerUserId))
                throw new InvalidOperationException("Failed to update resource.");
            if (request.TotalQuantity != existingUnits.Count)
            {
                var resizedUnits = existingUnits.Take(request.TotalQuantity)
                    .Select(unit => new DTO_OwnerResourceUnitUpdate
                    {
                        ResourceUnitId = unit.ResourceUnitId > 0 ? unit.ResourceUnitId : null,
                        Label = unit.Label,
                        Status = unit.Status
                    }).ToList();
                var existingLabels = resizedUnits.Select(unit => unit.Label).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var nextLabel = 1;
                while (resizedUnits.Count < request.TotalQuantity)
                {
                    var label = $"Unit {nextLabel++}";
                    if (existingLabels.Add(label))
                        resizedUnits.Add(new DTO_OwnerResourceUnitUpdate { Label = label, Status = "Available" });
                }
                await _ownerDLL.UpdateOwnerResourceUnits(centerId, resourceId, ownerUserId, resizedUnits);
            }
            return await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId)
                ?? throw new KeyNotFoundException("Resource not found.");
        }

        public async Task<bool> DeleteOwnerResource(int centerId, int resourceId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId) == null)
                throw new KeyNotFoundException("Resource not found.");
            if (!await _ownerDLL.ArchiveOwnerResource(centerId, resourceId, ownerUserId))
                throw new InvalidOperationException("Failed to archive resource.");
            return true;
        }

        public async Task<DTO_OwnerResource> UpdateOwnerResourceUnits(int centerId, int resourceId, DTO_OwnerResourceUnitsUpdate request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId) == null)
                throw new KeyNotFoundException("Resource not found.");
            if (request.Units.Select(unit => unit.Label.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.Units.Count)
                throw new ArgumentException("Resource unit labels must be unique.");

            if (!await _ownerDLL.UpdateOwnerResourceUnits(centerId, resourceId, ownerUserId, request.Units))
                throw new InvalidOperationException("Failed to update resource units.");
            return await _ownerDLL.GetOwnerResource(centerId, resourceId, ownerUserId)
                ?? throw new KeyNotFoundException("Resource not found.");
        }

        public async Task<List<DTO_OwnerService>> GetOwnerServices(int centerId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            return await _ownerDLL.GetOwnerServices(centerId, ownerUserId);
        }

        public async Task<DTO_OwnerService> CreateOwnerService(int centerId, DTO_AddService request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Service name is required.");
            return await _ownerDLL.AddOwnerService(centerId, ownerUserId, request.Name, request.Description, request.Icon, request.ImageUrl, request.IsActive);
        }

        public async Task<DTO_OwnerService> UpdateOwnerService(int centerId, int serviceId, DTO_UpdateService request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (serviceId is < 1 or > byte.MaxValue)
                throw new ArgumentException("Invalid service ID.");
            if (await _ownerDLL.GetOwnerService(centerId, serviceId, ownerUserId) == null)
                throw new KeyNotFoundException("Service not found.");

            request.CenterId = centerId;
            request.ServiceId = (byte)serviceId;
            if (!await UpdateService(request, ownerUserId))
                throw new InvalidOperationException("Failed to update service.");
            return await _ownerDLL.GetOwnerService(centerId, serviceId, ownerUserId)
                ?? throw new KeyNotFoundException("Service not found.");
        }

        public async Task<bool> DeleteOwnerService(int centerId, int serviceId, int ownerUserId)
        {
            await EnsureOwnerCenter(centerId, ownerUserId);
            if (serviceId is < 1 or > byte.MaxValue)
                throw new ArgumentException("Invalid service ID.");
            if (await _ownerDLL.GetOwnerService(centerId, serviceId, ownerUserId) == null)
                throw new KeyNotFoundException("Service not found.");
            if (!await DeleteService(serviceId, centerId, ownerUserId))
                throw new InvalidOperationException("Failed to delete service.");
            return true;
        }

        public async Task<List<DTO_BookingDetails>> GetOwnerBookings(int ownerUserId, DTO_OwnerBookingsQuery query)
        {
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (query.CenterId.HasValue)
                await EnsureOwnerCenter(query.CenterId.Value, ownerUserId);
            return await _ownerDLL.GetOwnerBookings(ownerUserId, query);
        }

        public async Task<DTO_BookingDetails> GetOwnerBooking(int bookingId, int ownerUserId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");
            return await _ownerDLL.GetOwnerBooking(bookingId, ownerUserId)
                ?? throw new KeyNotFoundException("Booking not found.");
        }

        public async Task<DTO_BookingDetails> UpdateOwnerBookingStatus(int bookingId, DTO_OwnerBookingStatusUpdate request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            var booking = await GetOwnerBooking(bookingId, ownerUserId);
            var requestedStatus = request.Status.Trim();
            var storedStatus = requestedStatus.ToLowerInvariant() switch
            {
                "confirmed" => "accepted",
                "pending" => "pending",
                "completed" => "completed",
                "cancelled" => "cancelled",
                _ => throw new ArgumentException("Unsupported booking status.")
            };
            var currentStatus = booking.Status.ToLowerInvariant();
            if (currentStatus == storedStatus || (storedStatus == "accepted" && currentStatus == "confirmed"))
                return booking;
            if (!(currentStatus == "pending" && storedStatus is "accepted" or "cancelled") && !(currentStatus == "accepted" && storedStatus == "completed"))
                throw new InvalidOperationException("The requested booking status transition is not allowed.");

            if (!await _ownerDLL.UpdateOwnerBookingStatus(bookingId, ownerUserId, storedStatus))
                throw new InvalidOperationException("Failed to update booking status.");
            return await GetOwnerBooking(bookingId, ownerUserId);
        }

        public Task<List<DTO_OwnerCustomerRequest>> GetOwnerRequests(int ownerUserId, string? type, string? status)
        {
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");
            return _ownerDLL.GetOwnerRequests(ownerUserId, type, status);
        }

        public async Task<DTO_OwnerCustomerRequest> GetOwnerRequest(int requestId, int ownerUserId)
        {
            if (requestId <= 0)
                throw new ArgumentException("Invalid request ID.");
            return await _ownerDLL.GetOwnerRequest(requestId, ownerUserId)
                ?? throw new KeyNotFoundException("Customer request not found.");
        }

        public async Task<DTO_OwnerCustomerRequest> UpdateOwnerRequestStatus(int requestId, DTO_OwnerRequestStatusUpdate request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            _ = await GetOwnerRequest(requestId, ownerUserId);
            var status = request.Status.Trim();
            if (!status.Equals("Accepted", StringComparison.OrdinalIgnoreCase) && !status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Request status must be Accepted or Rejected.");
            if (!await _ownerDLL.UpdateOwnerRequestStatus(requestId, ownerUserId, status))
                throw new InvalidOperationException("Failed to update customer request status.");
            return await GetOwnerRequest(requestId, ownerUserId);
        }

        public async Task<List<DTO_OwnerEmployee>> GetOwnerEmployees(int ownerUserId, int? centerId)
        {
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");
            if (centerId.HasValue)
                await EnsureOwnerCenter(centerId.Value, ownerUserId);
            return await _ownerDLL.GetOwnerEmployees(ownerUserId, centerId);
        }

        public async Task<DTO_OwnerEmployee> CreateOwnerEmployee(DTO_OwnerEmployeeCreateRequest request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            await EnsureOwnerCenter(request.CenterId, ownerUserId);
            if (await _user.ExistsByPhone(request.PhoneNumber))
                throw new InvalidOperationException("Phone number is already in use.");
            if (await _user.ExistsByEmail(request.Email))
                throw new InvalidOperationException("Email is already in use.");

            await using var transaction = await _ownerDLL.BeginTransactionAsync();
            try
            {
                var roleId = await _ownerDLL.GetOrCreateOwnerStaffRole(request.CenterId, ownerUserId, request.Role, request.Permissions);
                var userId = await _auth.RegisterWithRoleAsync(new UserRegisterRequest
                {
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Password = request.Password,
                    Email = request.Email,
                    CityId = request.CityId
                }, "Staff");
                var staffId = await _ownerDLL.AddOwnerEmployee(userId, request.CenterId, roleId, ownerUserId);
                await transaction.CommitAsync();
                return await _ownerDLL.GetOwnerEmployee(staffId, ownerUserId)
                    ?? throw new InvalidOperationException("The created employee could not be retrieved.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DTO_OwnerEmployee> UpdateOwnerEmployee(int employeeId, DTO_OwnerEmployeeUpdateRequest request, int ownerUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            var existing = await _ownerDLL.GetOwnerEmployee(employeeId, ownerUserId)
                ?? throw new KeyNotFoundException("Employee not found.");
            await EnsureOwnerCenter(request.CenterId, ownerUserId);
            if (await _user.ExistsByPhone(request.PhoneNumber, existing.UserId))
                throw new InvalidOperationException("Phone number is already in use.");
            if (await _user.ExistsByEmail(request.Email, existing.UserId))
                throw new InvalidOperationException("Email is already in use.");

            await using var transaction = await _ownerDLL.BeginTransactionAsync();
            try
            {
                var profile = await _user.GetUserInfoByID(existing.UserId)
                    ?? throw new KeyNotFoundException("Employee user not found.");
                var userUpdated = await _user.Update(existing.UserId, new DTO_UpdateUserInfoRequest
                {
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Company = profile.Company,
                    CityId = request.CityId
                });
                if (!userUpdated)
                    throw new ArgumentException(_user.LastError ?? "Employee details are invalid.");

                var roleId = await _ownerDLL.GetOrCreateOwnerStaffRole(request.CenterId, ownerUserId, request.Role, request.Permissions);
                if (!await _ownerDLL.UpdateOwnerEmployee(employeeId, ownerUserId, request.CenterId, roleId, request.FullName, request.PhoneNumber, request.Email, request.CityId, request.IsActive))
                    throw new InvalidOperationException("Failed to update employee.");
                await transaction.CommitAsync();
                return await _ownerDLL.GetOwnerEmployee(employeeId, ownerUserId)
                    ?? throw new KeyNotFoundException("Employee not found.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteOwnerEmployee(int employeeId, int ownerUserId)
        {
            if (!await _ownerDLL.ArchiveOwnerEmployee(employeeId, ownerUserId))
                throw new KeyNotFoundException("Employee not found.");
            return true;
        }

        public async Task<bool> ChangeOwnerPassword(int ownerUserId, ChangePasswordRequest request)
        {
            var result = await _auth.ChangePassword(ownerUserId, request);
            if (result.Success)
                return true;
            if (result.StatusCode == 400)
                throw new ArgumentException(result.Message);
            if (result.StatusCode == 404)
                throw new KeyNotFoundException(result.Message);
            if (result.StatusCode == 403)
                throw new UnauthorizedAccessException(result.Message);
            throw new InvalidOperationException(result.Message);
        }

        private async Task EnsureOwnerCenter(int centerId, int ownerUserId)
        {
            if (centerId <= 0 || ownerUserId <= 0)
                throw new ArgumentException("Invalid owner or center ID.");
            if (await _ownerDLL.GetOwnerCenter(centerId, ownerUserId) == null)
                throw new KeyNotFoundException("Center not found.");
        }


        /*// ================ Offers Management ================
        public async Task<bool> AddOffer()
        {

        }
        public async Task<bool> UpdateOffer()
        {

        }
        public async Task<bool> ActiveOrDisableOffer()
        {

        }
        public async Task<bool> DeleteOffer()
        {

        }
        public async Task<List<Tournament>> GetAllOffers()
        {

        }
        public async Task<List<Tournament>> GetAllOffers_ByCenterId(int centerId)
        {

        }
        // ================ Offers Management ================


        // ================ Tournaments Management ================
        public async Task<bool> AddTournament()
        {

        }
        public async Task<bool> UpdateTournament()
        {

        }
        public async Task<bool> ActiveOrDisableTournament()
        {

        }
        public async Task<bool> DeleteTournament()
        {

        }
        public async Task<List<Tournament>> GetAllTournaments()
        {

        }
        public async Task<List<Tournament>> GetAllTournaments_ByCenterId(int centerId)
        {

        }
        // ================ Tournaments Management ================
        
         
        // ================ View Reports ================
        public async Task<BookingReports> BookingReports()
        {

        }
        public async Task<DeviceUsageReports> DeviceUsageReports()
        {

        }
        public async Task<CustomerActivityReports> CustomerActivityReports ()
        {

        }
        public async Task<List<RatingReports>> RatingReports()
        {

        }
        // ================ View Reports ================


        // ================ Revenue Management ================
        public async Task<TotalRevenue> TotalRevenue()
        {

        }
        public async Task<RevenueByDevice> RevenueByDevice()
        {

        }
        public async Task<RevenueByPeriod> RevenueByPeriod ()
        {

        }
        public async Task<MostProfitableSessions> MostProfitableSessions()
        {

        }
        // ================ Revenue Management ================ 
        */
    }
}
