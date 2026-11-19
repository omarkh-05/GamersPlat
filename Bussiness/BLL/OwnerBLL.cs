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
    using Domain.DTOs.Invoice;
    using System.Net.NetworkInformation;

    public class OwnerBLL : IOwnerService
    {
        readonly IUser _user;
        readonly CenterBLL _center;
        readonly CenterResourcesBLL _centerResources;
        readonly CenterServiceBLL _centerServices;
        readonly BookingBLL _booking;
        readonly StaffBLL _staff;
        readonly IAuthService _auth;
        private readonly OwnerDLL _ownerDLL;
        private readonly InvoiceBLL _invoice;

        public OwnerBLL(IUser user, CenterBLL centerBLL, CenterResourcesBLL centerResourcesBLL, CenterServiceBLL centerServicesBLL, BookingBLL bookingBLL, StaffBLL staffBLL, IAuthService auth, OwnerDLL ownerDLL, InvoiceBLL invoice)
        {
            _user = user;
            _center = centerBLL;
            _centerResources = centerResourcesBLL;
            _centerServices = centerServicesBLL;
            _booking = bookingBLL;
            _staff = staffBLL;
            _auth = auth;
            _ownerDLL = ownerDLL;
            _invoice = invoice;
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
        public async Task<bool> AddCenter(DTO_CreateCenter addCenter,int ownerId)
        {
            if(addCenter == null)
            {
                throw new ArgumentNullException(nameof(addCenter), "Center cannot be null While Creating Center");
            }
            bool res = await _center.Add(addCenter, ownerId);
            if (res)
                return true;
            throw new ArgumentException("Error while adding center.");
        }
        public async Task<bool> UpdateCenter(DTO_UpdateCenter updateCenter,int centerId, int ownerId)
        {
            bool res = await _center.Update(updateCenter, centerId, ownerId);
            if (res)
                return true;
            throw new ArgumentException("Error while updating center.");
        }
        public async Task<bool> ActiveCenter(int centerId, int ownerUserId)
        {
            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool res = await _center.ActiveCenter(centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while active center.");
        }
        public async Task<bool> DeactiveCenter(int centerId, int ownerUserId)
        {
            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool res = await _center.DeactiveCenter(centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while deactive center.");
        }
        public async Task<bool> ToggleCenterStatus(int centerId, int ownerUserId, string status)
        {
            await ValidateCenterOwner(centerId, ownerUserId);

            if (status != "open" && status != "closed" && status != "busy" && status != "maintenance")
                throw new ArgumentException("Invalid status. Status must be either 'Open' or 'Closed' or 'Busy' or 'Maintenance'.");
            bool res = await _center.ToggleCenterStatus(centerId, status, ownerUserId);
            if (res)
                return true;
            throw new ArgumentException("Error while changing center status.");
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

            var res = await _center.GetCenterOverView(centerId);
            if (res != null)
                return res;

            throw new ArgumentException("Error while getting center overview.");
        }
        // ================ Center Management ================


        // ================ Resources Management ================
        public async Task<List<DTO_ResourceInfo>> GetAllResourceByCenterId(int centerId, int ownerUserId)
        {
            if (centerId <= 0)
            {
                throw new ArgumentException("Invalid center ID.");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            var resources = await _centerResources.GetAllByCenterId(centerId);
            if (resources == null || resources.Count == 0)
            {
                throw new InvalidOperationException("No resources found for this center.");
            }
            return resources;
        }
        public async Task<bool> AddResourceToCenter(DTO_AddResource resource,int centerId,int ownerUserId)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource), "Resource cannot be null while adding resource");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            var newResource = new CenterResource
            {
                CenterId = centerId,
                ResourcesTypeId = resource.ResourceId,
                HourlyPrice = resource.HourlyPrice,
                TotalQuantity = resource.TotalQuantity,
                RoomType = resource.RoomType ?? "normal",
                Description = resource.Description ?? string.Empty,
                IsActive = resource.IsActive,
                CreatedAt = DateTime.Now,
            };

            bool result = await _centerResources.Add(newResource);
            if (result)
                return true;
            else
                throw new ArgumentNullException("Error while adding resource to center");
        }
        public async Task<bool> UpdateCenterResource(DTO_UpdateResource resource, int centerId, int centerResourceId, int ownerUserId)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource), "Resource cannot be null while adding resource");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool result = await _centerResources.Update(resource,centerId,centerResourceId);
            if (result)
                return true;
            else
                throw new ArgumentNullException("Error while updating center resource");
        }
        public async Task<bool> ActiveCenterResource( int centerId, int centerResourceId, int ownerUserId)
        {
            if (centerResourceId <= 0)
            {
                throw new ArgumentException("Invalid resource ID.");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool res = await _centerResources.ActiveStatus(centerId, centerResourceId);
            if (res)
                return true;
            throw new ArgumentException("Error while active resource.");
        }
        public async Task<bool> DeactiveCenterResource( int centerId ,int centerResourceId, int ownerUserId)
        {
            if (centerResourceId <= 0)
            {
                throw new ArgumentException("Invalid resource ID.");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool res = await _centerResources.DeactiveStatus(centerId, centerResourceId);
            if (res)
                return true;
            throw new ArgumentException("Error while deactive resource.");
        }
        // ================ Resource Management ================


        // ================ Services Management ================
        public async Task<List<DTO_ServicesInfo>> GetCenterServices(int centerId, int ownerUserId)
        {
            if (centerId <= 0)
            {
                throw new ArgumentException("Invalid center ID.");
            }
            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);
            var services = await _centerServices.GetAllByCenterId(centerId);
            if (services == null || services.Count == 0)
            {
                throw new InvalidOperationException("No services found for this center.");
            }
            return services;
        }
        public async Task<bool> AddCenterService(DTO_AddCenterService service, int centerId, int ownerUserId)
        {
            if (service == null)
            {
                throw new ArgumentNullException( nameof(service),"Service cannot be null while adding service");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            var newService = new CenterService
            {
                ServiceId = service.ServiceId,
                CenterId = centerId,
                Icon = service.Icon,
                IsActive = service.IsActive,
                CreatedAt = DateTime.Now
            };

            bool result = await _centerServices.Add(newService, centerId);

            if (result)
                return true;
            else
                throw new ArgumentNullException("Error while adding service to center");
        }
        public async Task<bool> UpdateCenterService(DTO_UpdateCenterService service, int centerId, int centerServiceId, int ownerUserId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service),"Service cannot be null while updating service");
            }

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

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
        public async Task<bool> ActiveCenterService(int centerId, int centerServiceId, int ownerUserId)
        {
            if (centerServiceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool res = await _centerServices.ActiveStatus(centerServiceId, centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while active service.");
        }
        public async Task<bool> DeactiveCenterService(int centerId, int centerServiceId, int ownerUserId)
        {
            if (centerServiceId <= 0)
                throw new ArgumentException("Invalid service ID.");

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

            bool res = await _centerServices.DeactiveStatus(centerServiceId, centerId);
            if (res)
                return true;
            throw new ArgumentException("Error while deactive service.");
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
        public async Task<bool> AcceptBooking(int bookingId,int ownerId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await ValidateCenterOwner(booking.CenterId, ownerId, requireApproved: true);

            if (booking.Status != "pending" && booking.Status != "updated")
                throw new InvalidOperationException("Only pending or updated bookings can be accepted.");

            bool res = await _booking.AcceptBooking(bookingId, ownerId);
            if (res)
                return true;
            throw new ArgumentException("Error while accept booking.");
        }
        public async Task<bool> RejectBooking(int bookingId,int ownerId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await ValidateCenterOwner(booking.CenterId, ownerId, requireApproved: true);

            if (booking.Status != "pending" && booking.Status != "updated")
                throw new InvalidOperationException("Only pending or updated bookings can be rejected.");

            bool res = await _booking.RejectBooking(bookingId, ownerId);
            if (res)
                return true;
            throw new ArgumentException("Error while reject booking.");
        }
        public async Task<bool> StartBooking(int bookingId, int ownerId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await ValidateCenterOwner(booking.CenterId, ownerId, requireApproved: true);

            if (booking.Status != "accepted")
                throw new InvalidOperationException("Only accepted bookings can be started.");

            bool res = await _booking.StartBooking(bookingId, ownerId);
            if (res)
                return true;
            throw new ArgumentException("Error while start booking.");
        }
        public async Task<DTO_InvoicePreview> GetInvoicePreview(int bookingId, int ownerId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await ValidateCenterOwner(booking.CenterId, ownerId, requireApproved: true);

            return await _booking.GetInvoicePreview(bookingId)
                ?? throw new InvalidOperationException("Could not create invoice preview.");
        }
        public async Task<DTO_Invoice> StopBooking(int bookingId, int ownerId, DTO_EndBookingInvoice endBooking)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await ValidateCenterOwner(booking.CenterId, ownerId, requireApproved: true);

            var res = await _booking.StopBooking(bookingId, ownerId, endBooking);
            if (res != null)
                return res;

            throw new ArgumentException("Error while stop booking.");
        }

        // Todo : implement Start, End Booking Logic
        /*public async Task<bool> StartBooking(int bookingId, int ownerUserId)
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

            if (booking.Status != "approved")
                throw new InvalidOperationException("Only approved bookings can be start.");

            return await _booking.StartBooking(bookingId);
        }
        public async Task<bool> EndBooking(int bookingId, int ownerUserId)
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

            if (booking.Status != "running")
                throw new InvalidOperationException("Only approved bookings can be start.");

            return await _booking.EndBooking(bookingId);
        }*/
        public async Task<DTO_BookingDetails> ViewBookingDetails(int bookingId,int ownerUserId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _booking.GetByID(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            await ValidateCenterOwner(booking.CenterId, ownerUserId, requireApproved: true);

            var bookingDetails = new DTO_BookingDetails
            {
                CustomerName = booking.CustomerName,
                CenterName = booking.Center.CenterName,
                PhoneNumber = booking.PhoneNumber,
                ResourcesType = booking.CenterResource.ResourcesType.ResourceName,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime ,
                Duration = $"{(booking.EndTime - booking.StartTime).TotalMinutes} Minutes" ,
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

            await ValidateCenterOwner(centerId, ownerUserId, requireApproved: true);

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
            if(result > 0)
            {
                return true;
            }
            throw new ArgumentException("Error while adding staff");
        }
        public async Task<bool> UpdateStaffMember(DTO_UpdateStaffMember staffMember)
        {
            if (staffMember == null)
            {
                throw new ArgumentNullException(nameof(staffMember), "Staff member cannot be null while updating staff member");
            }
            var result = await _staff.Update(staffMember);
            if (result)
            {
                return true;
            }
            throw new ArgumentException("Error while updating staff");
        }
        public async Task<bool> DeleteStaffMember(int userId)
        {
            return await _staff.Delete(userId);
        }
        public async Task<List<DTO_StaffDetails>> GetAllStaffByOwnerId(int ownerId)
        {
            var center = await _center.GetAllCentersByOwnerId(ownerId);

            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            return await _staff.GetAll();
        }
        public async Task<List<DTO_StaffDetails>> GetAllStaffByCenterId(int centerId)
        {
            return await _staff.GetAllStaff_ByCenterId(centerId);
        }
        // ================ Staffs Management ================


        // ================ Services Management ================
        public async Task<List<DTO_CenterInvoices>?> GetAllCenterInvoices(int centerId,int ownerId)
        {
            await ValidateCenterOwner(centerId, ownerId, requireApproved: true);

            var res = await _invoice.GetAllCenterInvoices(centerId);
            if (res != null)
                return res;
            throw new KeyNotFoundException("No invoices found");
        }
        // ================ Services Management ================


        // Helpers
        private async Task ValidateCenterOwner(int centerId, int ownerId, bool requireApproved = false)
        {
            if(centerId<=0 || ownerId <= 0)
                throw new ArgumentException("Invalid Input Data");

            var center = await _center.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            if (requireApproved && center.IsApproved != true)
                throw new ArgumentException("Can not change status of an unapproved center");
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
