using Azure.Core;
using Bussiness.Enums;
using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Domain.DTOs.Invoice;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.Staff;

namespace Bussiness.BLL
{
    public class StaffBLL
    {
        readonly IAuthService _auth;
        readonly ICenterService _center;
        readonly IBookingService _booking;
        readonly IUser _user;
        readonly IUserRole _userRole;
        private readonly IRoles _roles;
        readonly StaffDLL _staffDLL;
        readonly CenterResourcesBLL _centerResources;
        readonly CenterServiceBLL _centerServices;
        readonly GamersPlatDbContext _db;
        public StaffBLL(StaffDLL staffDLL, IAuthService auth, ICenterService center, IBookingService booking, IUser user, IUserRole userRole, IRoles roles, CenterResourcesBLL centerResources, CenterServiceBLL centerServices,  GamersPlatDbContext db)
        {
            _staffDLL = staffDLL;
            _auth = auth;
            _user = user;
            _userRole = userRole;
            _roles = roles;
            _center = center;
            _centerResources = centerResources;
            _centerServices = centerServices;
            _booking = booking;
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(DTO_AddStaffMember staff)
        {
            if (staff == null)
                throw new ArgumentNullException(nameof(staff));

            if (string.IsNullOrWhiteSpace(staff.FullName) ||
                 string.IsNullOrWhiteSpace(staff.PhoneNumber) ||
                 string.IsNullOrWhiteSpace(staff.PasswordHash))
                throw new Exception("Invalid data");

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (await _user.ExistsByPhone(staff.PhoneNumber))
                    throw new Exception("Phone number already exists");

                var role = await _roles.GetByName("staff");
                if (role == null)
                    throw new Exception("Invalid role");

                var user = new User
                {
                    FullName = staff.FullName,
                    PhoneNumber = staff.PhoneNumber,
                    Email = staff.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(staff.PasswordHash),
                    IsActive = true,
                    CityId = staff.CityId
                };

                if (!await _user.Add(user))
                    throw new Exception("Failed to create user");

                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId,
                };

                if (!await _userRole.Add(userRole))
                    throw new Exception("Failed to add user role");

                var staffEntity = new Staff
                {
                    UserId = user.UserId,
                    CenterId = staff.CenterId,
                    Permissions = staff.Permissions,
                    RoleName = staff.RoleName ?? string.Empty,
                    IsActive = staff.IsActive,
                    Status = "active"
                };

                var staffId = await _staffDLL.Add(staffEntity);

                if (staffId < 1)
                    throw new Exception("Failed to add staff");

                await transaction.CommitAsync();
                return staffId;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                EventLog_Helper.WriteEventLog("Add Staff Error", ex);
                throw;
            }
        }
        public async Task<bool> Update(DTO_UpdateStaffMember staff) => await _staffDLL.Update(staff);
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


        // ================ Profile ================
        public async Task<bool> ChangeStaffPassword(int userId,ChangePasswordRequest request)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid staff ID.");

            var result = await _auth.ChangePassword(userId, request);

            if (!result.Success)
                throw new InvalidOperationException(result.Message);

            return true;
        }
        // ================ Profile ================


        // ================ Resources Management ================
        public async Task<List<DTO_ResourceInfo>> GetAllResourceByCenterId( int centerId, int staffId)
        {
            await GetAuthorizedStaff(staffId, centerId, Permissions.View);

            var resources = await _centerResources.GetAllByCenterId(centerId);

            if (resources == null || resources.Count == 0)
                throw new InvalidOperationException("No resources found for this center.");

            return resources;
        }
        public async Task<bool> AddResourceToCenter(DTO_AddResource resource, int centerId, int staffId)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            await GetAuthorizedStaff(staffId, centerId, Permissions.Add);

            var newResource = new CenterResource
            {
                CenterId = centerId,
                ResourcesTypeId = resource.ResourceId,
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                RoomType = resource.RoomType ?? "normal",
                Description = resource.Description ?? string.Empty,
                IsActive = resource.IsActive
            };

            bool res = await _centerResources.Add(newResource);
            if (res)
                return true;
            throw new ArgumentException("Error while adding resource.");
        }
        public async Task<bool> UpdateCenterResource(DTO_UpdateResource resource, int centerId, int centerResourceId, int staffId)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            await GetAuthorizedStaff(staffId, centerId, Permissions.Edit);

            bool res = await _centerResources.Update(resource, centerId, centerResourceId);
            if (res)
                return true;
            throw new ArgumentException("Error while updating resource.");
        }
        public async Task<bool> ActiveCenterResource(int centerId, int centerResourceId, int staffId)
        {
            if (centerResourceId <= 0)
                throw new ArgumentException("Invalid resource ID.");

            await GetAuthorizedStaff(staffId, centerId, Permissions.Edit);
            bool res = await _centerResources.ActiveStatus(centerId, centerResourceId);
            if (res)
                return true;
            throw new ArgumentException("Error while active resource.");
        }
        public async Task<bool> DeactiveCenterResource(int centerId, int centerResourceId, int staffId)
        {
            if (centerResourceId <= 0)
                throw new ArgumentException("Invalid resource ID.");

            await GetAuthorizedStaff(staffId, centerId, Permissions.Edit);

            bool res = await _centerResources.DeactiveStatus(centerId, centerResourceId);
            if (res)
                return true;
            throw new ArgumentException("Error while deactive resource.");
        }
        // ================ Resource Management ================


        // ================ Services Management ================
        public async Task<List<DTO_ServicesInfo>> GetCenterServices(int centerId, int staffId)
        {
            await GetAuthorizedStaff(staffId, centerId, Permissions.View);
            var services = await _centerServices.GetAllByCenterId(centerId);
            if (services == null || services.Count == 0)
                throw new InvalidOperationException("No services found for this center.");
            return services;
        }
        public async Task<bool> AddCenterService(DTO_AddCenterService service, int centerId, int staffId)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            await GetAuthorizedStaff(staffId, centerId, Permissions.Add);

            var newService = new CenterService
            {
                ServiceId = service.ServiceId,
                CenterId = centerId,
                IsActive = service.IsActive,
                CreatedAt = DateTime.Now
            };

            bool res =  await _centerServices.Add(newService, centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while add service.");
        }
        public async Task<bool> UpdateCenterService(DTO_UpdateCenterService service, int centerId, int centerServiceId, int staffId)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            await GetAuthorizedStaff(staffId, centerId, Permissions.Edit);

            var centerService = await _centerServices.GetByID(centerServiceId);
            if (centerService == null)
                throw new KeyNotFoundException("Center service not found.");

            if (centerService.CenterId != centerId)
                throw new UnauthorizedAccessException("Center service does not belong to this center.");

            centerService.ServiceId = service.ServiceId;
            centerService.Icon = service.Icon;
            centerService.IsActive = service.IsActive;

            bool result = await _centerServices.Update(centerService);

            if (result)
                return true;
            else
                throw new ArgumentNullException("Error while updating center service");
        }
        public async Task<bool> ActiveCenterService(int centerId, int centerServiceId, int staffId)
        {
            if (centerServiceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            await GetAuthorizedStaff(staffId, centerId, Permissions.Edit);

            bool res = await _centerServices.ActiveStatus(centerServiceId, centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while active service.");
        }
        public async Task<bool> DeactiveCenterService(int centerId, int centerServiceId, int staffId)
        {
            if (centerServiceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            await GetAuthorizedStaff(staffId, centerId, Permissions.Edit);

            bool res = await _centerServices.DeactiveStatus(centerServiceId, centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while deactive service.");
        }
        // ================ Services Management ================


        // ================ Bookings Management ================
        public async Task<DTO_BookingDetails> ViewBookingDetails(int bookingId, int staffId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(
                staffId, booking.CenterId, Permissions.View);

            var bookingDetails = new DTO_BookingDetails
            {
                CustomerName = booking.CustomerName,
                CenterName = booking.Center.CenterName,
                PhoneNumber = booking.PhoneNumber,
                ResourcesType = booking.CenterResource.ResourcesType.ResourceName,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Duration = $"{(booking.EndTime - booking.StartTime).TotalMinutes} Minutes",
                TotalPrice = booking.TotalPrice,
                Status = booking.Status
            };

            return bookingDetails;
        }
        public async Task<List<DTO_BookingDetails>> GetAllBookings_ByCenterId(int centerId, int staffId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            await GetAuthorizedStaff(staffId, centerId, Permissions.View);

            return await _booking.GetBookingsByCenterId(centerId);
        }
        public async Task<bool> AddBooking(DTO_BookingBase booking, int staffId)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            var staff = await GetAuthorizedStaff(staffId, booking.CenterId, Permissions.Add);

            return await _booking.Add(booking, staff.StaffId, null, null);
        }
        public async Task<bool> UpdateBooking(DTO_UpdateBooking booking, int bookingId, int staffId)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var existingBooking = await _booking.GetByID(bookingId);

            if (existingBooking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(staffId, existingBooking.CenterId, Permissions.Edit);

            return await _booking.Update(booking, 0);
        }
        public async Task<bool> AcceptBooking(int bookingId, int staffId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(staffId, booking.CenterId, Permissions.AcceptRejectBooking);

            if (booking.Status != "pending" && booking.Status != "updated")
                throw new InvalidOperationException("Only pending bookings can be accepted.");

            bool res =  await _booking.AcceptBooking(bookingId, staffId);
            if (res)
                return true;
            throw new ArgumentException("Error while accept booking.");

        }
        public async Task<bool> RejectBooking(int bookingId, int staffId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(staffId, booking.CenterId, Permissions.AcceptRejectBooking);

            if (booking.Status != "pending" && booking.Status != "updated")
                throw new InvalidOperationException("Only pending bookings can be rejected.");

            bool res = await _booking.RejectBooking(bookingId, staffId);
            if (res)
                return true;
            throw new ArgumentException("Error while reject booking.");
        }
        public async Task<bool> StartBooking(int bookingId, int staffId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(staffId, booking.CenterId, Permissions.AcceptRejectBooking);

            if (booking.Status != "accepted")
                throw new InvalidOperationException("Only accepted bookings can be started.");

            bool res = await _booking.StartBooking(bookingId, staffId);
            if (res)
                return true;
            throw new ArgumentException("Error while start booking.");
        }
        public async Task<DTO_InvoicePreview> GetInvoicePreview(int bookingId, int staffId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(staffId, booking.CenterId, Permissions.AcceptRejectBooking);

            return await _booking.GetInvoicePreview(bookingId)
                ?? throw new InvalidOperationException("Could not create invoice preview.");
        }
        public async Task<DTO_Invoice> StopBooking(int bookingId, int staffId, DTO_EndBookingInvoice endBooking)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await GetAuthorizedStaff(staffId, booking.CenterId, Permissions.AcceptRejectBooking);

            var res = await _booking.StopBooking(bookingId, staffId, endBooking);
            if (res != null)
                return res;
            throw new ArgumentException("Error while stop booking.");
        }
        // ================ Bookings Management ================


        // ================ Helpers ================
        private async Task<DTO_StaffDetails> GetAuthorizedStaff(int userId, int centerId, Permissions requiredPermission)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid staff ID.");

            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            var staff = await _staffDLL.GetByID(userId);

            if (staff == null)
                throw new KeyNotFoundException("Staff member not found.");

            if (!staff.IsActive)
                throw new UnauthorizedAccessException("Staff account is inactive.");

            if (staff.CenterId != centerId)
                throw new UnauthorizedAccessException("You are not authorized to access this center.");

            var center = await _center.GetByID(centerId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            var permissions = (Permissions)staff.Permissions;

            if ((permissions & requiredPermission) != requiredPermission)
                throw new UnauthorizedAccessException("You don't have permission to perform this operation.");

            return staff;
        }
        // ================ Helpers ================
    }
}
