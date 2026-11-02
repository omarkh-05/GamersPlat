using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Owner;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.Staff;
using Domain.DTOs.User;

namespace Bussiness.BLL
{
    using Bussiness.Interfaces;
    using DataLayer;
    using Domain.DTOs.Auth;

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
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }
        public async Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId)
        {
            if (ownerId <= 0)
            {
                throw new ArgumentException("Invalid owner ID.");
            }
            var ownerDashboard = await _ownerDLL.GetOwnerDashboard(ownerId);
            if(ownerDashboard == null)
            {
                throw new InvalidOperationException("Owner dashboard not found.");
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
                throw new InvalidOperationException("Failed to update user information.");
            }
            return userUpdated;
        }
        public async Task<bool?> ChangeOwnerPassword(int ownerId, ChangePasswordRequest changePasswordRequest)
        {
            if (ownerId <= 0)
            {
                throw new ArgumentException("Invalid owner ID.");
            }
            var passwordChanged = await _auth.ChangePassword(ownerId, changePasswordRequest);
            if (passwordChanged == null)
            {
                throw new InvalidOperationException("Failed to change password.");
            }
            return true;
        }
        // ================ Owner Management ================


        // ================ Center Management ================
        public async Task<bool> AddCenter(DTO_CreateCenter addCenter)
        {
            if(addCenter == null)
            {
                throw new ArgumentNullException(nameof(addCenter), "Center cannot be null While Creating Center");
            }
            return await _center.Add(addCenter, 1);
        }
        public async Task<bool> UpdateCenter(DTO_UpdateCenter updateCenter, int ownerUserId)
        {
            if (updateCenter == null)
            {
                throw new ArgumentNullException(nameof(updateCenter), "Center cannot be null While Updating Center");
            }
            var center = await _center.GetByID(updateCenter.CenterId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _center.Update(updateCenter, ownerUserId);
        }
        public async Task<bool> ActiveCenter(int centerId, int ownerUserId)
        {
            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _center.ActiveCenter(centerId);
        }
        public async Task<bool> DeactiveCenter(int centerId, int ownerUserId)
        {
            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _center.DeactiveCenter(centerId);
        }
        public async Task<bool> ToggleCenterStatus(int centerId, int ownerUserId, string status)
        {
            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");
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
            var centers = await _center.GetAllCentersByOwnerId(ownerId);
            if(centers == null || centers.Count == 0)
            {
                throw new InvalidOperationException("No centers found for this owner.");
            }
            return centers;
        }
        public async Task<DTO_CenterOverview?> GetCenterOverView(int centerId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            return await _center.GetCenterOverView(centerId);
        }
        // ================ Center Management ================


        // ================ Resources Management ================
        public async Task<bool> AddResource(DTO_AddResource resource,int ownerUserId)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource), "Resource cannot be null while adding resource");
            }
            var center = await _center.GetByID(resource.CenterId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            var newResource = new ResourcesType
            {
                CenterId = resource.CenterId,
                DeviceId = resource.DeviceId,
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                RoomType = resource.RoomType,
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
            var center = await _center.GetByID(resource.CenterId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            var newResource = new ResourcesType
            {
                CenterId = resource.CenterId,
                DeviceId = resource.DeviceId,
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                RoomType = resource.RoomType,
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
            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _resourcesType.ActiveStatus(centerId, resourceId);
        }
        public async Task<bool> DeactiveResource(int resourceId, int centerId, int ownerUserId)
        {
            if (resourceId <= 0)
            {
                throw new ArgumentException("Invalid resource ID.");
            }
            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _resourcesType.DeactiveStatus(centerId, resourceId);
        }
        public async Task<List<DTO_ResourceInfo>> GetAllResourceByCenterId(int centerId, int ownerUserId)
        {
            if (centerId <= 0)
            {
                throw new ArgumentException("Invalid center ID.");
            }
            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");
            var resources = await _resourcesType.GetAllByCenterId(centerId);
            if (resources == null || resources.Count == 0)
            {
                throw new InvalidOperationException("No resources found for this center.");
            }
            return resources;
        }
        // ================ Resource Management ================


        // ================ Services Management ================
        public async Task<bool> AddService(DTO_AddService service, int ownerUserId)
        {
            if (service == null)
            {
                throw new ArgumentNullException( nameof(service),"Service cannot be null while adding service");
            }

            var center = await _center.GetByID(service.CenterId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException(
                    "You are not the owner of this center.");

            var newService = new Service
            {
                CenterId = service.CenterId,
                Name = service.Name,
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

            var center = await _center.GetByID(service.CenterId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException(
                    "You are not the owner of this center.");

            var updatedService = new Service
            {
                ServiceId = service.ServiceId,
                CenterId = service.CenterId,
                Name = service.Name,
                IsActive = service.IsActive,
                UpdatedAt = DateTime.UtcNow
            };

            return await _services.Update(updatedService);
        }
        public async Task<List<DTO_ServicesInfo>> GetAllServicesByCenterId(int centerId, int ownerUserId)
        {
            if (centerId <= 0)
            {
                throw new ArgumentException("Invalid center ID.");
            }
            var center = await _services.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.CenterId != centerId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");
            var resources = await _services.GetAllByCenterId(centerId);
            if (resources == null || resources.Count == 0)
            {
                throw new InvalidOperationException("No services found for this center.");
            }
            return resources;
        }
        public async Task<bool> ActiveService(int serviceId,int centerId,int ownerUserId)
        {
            if (serviceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            var center = await _center.GetByID(centerId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _services.ActiveStatus(serviceId, centerId);
        }
        public async Task<bool> DeactiveService(int serviceId, int centerId, int ownerUserId)
        {
            if (serviceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            var center = await _center.GetByID(centerId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException(
                    "You are not the owner of this center.");

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

            var center = await _center.GetByID(booking.CenterId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

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

            var center = await _center.GetByID(booking.CenterId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            if (booking.Status != "Pending")
                throw new InvalidOperationException("Only pending bookings can be rejected.");

            return await _booking.Accept_RejectBooking(bookingId,"Rejected");
        }
        public async Task<DTO_BookingDetails> ViewBookingDetails(int bookingId,int ownerUserId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            var center = await _center.GetByID(booking.CenterId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            var bookingDetails = new DTO_BookingDetails
            {
                CustomerName = booking.CustomerName,
                CenterName = booking.Center.CenterName,
                PhoneNumber = booking.PhoneNumber,
                ResourcesType = booking.ResourcesType.Device.DeviceName,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime.HasValue ? booking.EndTime.Value:booking.StartTime.AddHours(1),
                Duration = booking.EndTime.HasValue ? $"{(booking.EndTime.Value - booking.StartTime).TotalMinutes} Minutes" : "Open Time",
                TotalPrice = booking.TotalPrice,
                Status = booking.Status
            };

            return bookingDetails;
        }
        public async Task<List<DTO_BookingDetails>> GetAllBookings(int ownerUserId)
        {
            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner ID.");

            return await _booking.GetBookingsByOwnerId(ownerUserId);
        }
        public async Task<List<DTO_BookingDetails>> GetAllUBookings_ByCenterId(int centerId,int ownerUserId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            var center = await _center.GetByID(centerId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerUserId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            return await _booking.GetBookingsByCenterId(centerId);
        }
        // ================ Bookings Management ================


        // ================ Staffs Management ================
        public async Task<bool> AddStaffMember(DTO_AddStaffMember staffMember)
        {
            if (staffMember == null)
            {
                throw new ArgumentNullException(nameof(staffMember), "Staff member cannot be null while adding staff member");
            }
           var result = await _staff.Add(staffMember);
            if(result)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateStaffMember(int userId,DTO_UpdateStaffMember request)
        {
            return true;
        }
        public async Task<bool> DeleteStaffMember(int userId)
        {
            return await _staff.Delete(userId);
        }
        public async Task<bool> AddStaffRoleByCenterId(DTO_StaffRole role)
        {
            return await _staffRole.AddStaffRole(role);
        }
        public async Task<bool> SetRoleToMember(DTO_SetRoleToMember request)
        {
            return await _staffRole.SetRoleToMember(request);
        }
        public async Task<List<DTO_StaffDetails>> GetAllStaffByOwnerId(int ownerId)
        {
            return await _staff.GetAll();
        }
        public async Task<List<DTO_StaffDetails>> GetAllStaffByCenterId(int centerId)
        {
            return await _staff.GetAllStaff_ByCenterId(centerId);
        }
        // ================ Staffs Management ================


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
