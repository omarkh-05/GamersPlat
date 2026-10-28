# Owner REST API endpoint specification

This is a backend proposal based on the owner UI in `src/app/(public)/owner/page.tsx` and its owner sections. It does not change the frontend. The frontend currently uses mock/in-memory data for these sections, so the routes below describe the API contract to replace that data when the frontend is connected.

The route prefix follows the attached controller example: `api/[controller]`. With `OwnerController`, the full prefix is `/api/Owner`. Protect owner routes with `[Authorize(Roles = "Owner")]`. Keep data access and business rules in the BLL/service layer.

## Important: integrate with the existing backend first

The attached BLL snapshot already has an owner service. Do not create a second `IOwner`, `OwnerBLL`, DTO set, or data-access layer just to match the route names in this document. The route outline below describes the HTTP contract; it is not permission to duplicate backend types. Before implementing an endpoint, search the existing `Domain`, `Bussiness`, and `Data` projects for a DTO, interface method, BLL method, and DLL/repository operation with the same purpose.

Use the matching existing type when its fields and behavior fit the frontend. If a method exists but is missing an owner ID or ownership check, extend that method and its call chain rather than adding a parallel implementation. Add a new DTO/BLL method only when no existing one can provide the required owner-screen behavior. Add a new DLL/repository method only when the existing data-access methods cannot retrieve or persist the required data. Controllers call the owner service/BLL only; they must never call `OwnerDLL` or another data-access class directly.

### Existing backend types and operations in the supplied BLL

The snapshot defines `OwnerBLL : IOwnerService` in `Bussiness.BLL`. It already depends on the shared user, center, resource, service, booking, staff, staff-role, and authentication BLL/services, plus `OwnerDLL`. Inject the existing `IOwnerService` registered by the backend's dependency-injection setup (or use its established existing registration pattern); do not instantiate `OwnerBLL` or create a competing `IOwner` abstraction solely for these routes.

| Owner feature/route purpose | Reuse from the supplied backend | Integration note |
|---|---|---|
| Owner overview/dashboard | `OwnerBLL.GetOwnerDashboard(ownerId)` → `DTO_OwnerDashboard`; data is obtained through `OwnerDLL.GetOwnerDashboard(ownerId)`. | Use this existing aggregate. Add fields to the existing dashboard DTO/DLL query only if the owner UI actually needs fields it does not currently return. |
| Owner profile read/update | `GetOwnerInfo(ownerId)` → `DTO_UserInfoRequest`; `UpdateOwnerInfo(ownerId, DTO_UpdateUserInfoRequest)`. | Use these existing DTOs if they contain the profile fields required by the UI. Extend the existing DTO/update path for missing owner fields rather than creating duplicate profile DTOs. |
| Center list/create/update/status | `GetAllCentersForOwner(ownerId)` → `List<DTO_CentersListDetails>`; `AddCenter(DTO_CreateCenter)`; `UpdateCenter(DTO_UpdateCenter, ownerId)`; `Active_DeactiveCenter(centerId, ownerId)`; `ToggleCenterStatus(centerId, ownerId, status)`. | `AddCenter` currently passes the hard-coded owner ID `1` to `_center.Add`. Change the existing method to accept the authenticated `ownerId` and pass that through before exposing center creation. `Active_DeactiveCenter` currently calls `_center.ActiveCenter` even though its name implies both operations; correct/replace that existing behavior before wiring a deactivate action. `GetAllCentersForOwner` currently throws when the list is empty; for a collection API, prefer returning an empty list. There is no center-delete operation in the supplied owner BLL; add one only if center deletion/archive is required and supported by the existing center/data layer. |
| Resource types | `AddResource(DTO_AddResource, ownerId)`; `UpdateResource(DTO_UpdateResource, ownerId)`; `ActiveResource(resourceId, centerId, ownerId)`; `DeactiveResource(resourceId, centerId, ownerId)`. These already check that the owner owns the center. | Reuse these DTOs and methods. The snapshot does not show owner-scoped resource listing or deletion; search `ResourcesTypeBLL`/`Data` first, then add owner-scoped BLL methods and only missing persistence operations. |
| Center services | `AddService(DTO_AddService, ownerId)`; `UpdateService(DTO_UpdateService, ownerId)`; `DeleteService(serviceId, centerId, ownerId)`; `ActiveService(serviceId, centerId, ownerId)`; `DeactiveService(serviceId, centerId, ownerId)`. | These methods already verify center ownership. Reuse the existing service DTOs and methods; do not make replacement `OwnerServiceDto` types unless an API response mapping genuinely requires a distinct response shape. |
| Bookings | `GetAllBookings(ownerId)` → `List<DTO_BookingDetails>`; `GetAllUBookings_ByCenterId(centerId, ownerId)`; `ViewBookingDetails(bookingId, ownerId)`; `AcceptBooking(bookingId, ownerId)`; `RejectBooking(bookingId, ownerId)`. Existing input DTOs include `DTO_BookingBase` and `DTO_UpdateBooking`. | The read, center-scope, accept, and reject methods already contain owner checks. `UpdateBooking(DTO_UpdateBooking)` has no owner ID/check in the snapshot; do not expose it for owner changes until the existing method is made owner-scoped. The frontend calls statuses `Confirmed`/`Cancelled`, while this BLL accepts/rejects bookings as `Accepted`/`Rejected`; define an intentional mapping or align the existing business status model before exposing status updates. |
| Staff/employees | `AddStaffMember(DTO_AddStaffMember)`; `UpdateStaffMember(userId, DTO_UpdateUserInfoRequest)`; `DeleteStaffMember(userId)`; `AddStaffRole_ByCenterId(DTO_StaffRole)`; `SetRoleToMember(DTO_SetRoleToMember)`; `GetAllStaff()` / `GetAllStaff_ByCenterId(centerId)` → `DTO_StaffDetails`. | Reuse these DTOs and methods only after adding owner/center authorization where absent. The snapshot's staff methods do not accept `ownerId`, and `GetAllStaff()` is global; do not expose them directly to owners. Add owner-scoped wrappers or extend the existing methods and verify the center belongs to the authenticated owner. |
| Password change | Existing `IAuthService` dependency (`_auth`). | Reuse the existing auth password request and change method if present in the backend. Do not store or return passwords in owner DTOs. |
| Customer requests, workspace aggregate, center details, resource list/delete | No corresponding owner operation is shown in the supplied excerpt. | Search all existing BLL and DLL/repository code first. Add only the missing owner-scoped BLL methods; for requests/workspace, reuse underlying booking/resource/staff queries where available. Do not add controller-side aggregation or access the DLL directly. |

These are reuse targets, not a claim that the attached excerpt contains every backend project type or every method. Confirm the full solution before creating anything. The existing DTO names above are the names visible in the snapshot; where an endpoint needs a new request/response field, first extend a compatible existing DTO if that change will not break other API consumers. Otherwise add a narrowly scoped DTO in the existing `Domain.DTOs` namespace structure.

When a required operation is genuinely missing, add it within the existing architecture in this order:

1. Reuse an existing DTO and lower-level BLL method where possible.
2. If the owner-facing operation is missing, add its signature to the existing `IOwnerService` and implement it in `OwnerBLL`, keeping owner checks and orchestration there.
3. If persistence/data access is missing, add the smallest required method to the existing DLL/repository and call it through the BLL. Do not have a controller call the DLL.
4. Add a new DTO only if no existing DTO can safely represent the required request/response. Follow the established `DTO_*` naming and `Domain.DTOs.*` namespaces.
5. Reuse the current dependency-injection registration. Add/update registration only if the existing owner service is not already registered.

## Controller route outline

The code below lists proposed action names and routes only. Its `IOwner`, method, and `DTO_*` placeholders are not the names of the supplied `IOwnerService`/`OwnerBLL` operations. When implementing it, inject `IOwnerService`, use the existing DTOs and BLL operations in the table above, and add only the missing operations identified after checking the full backend. Route handlers should remain thin.

```csharp
using Bussiness.Interfaces;
using Domain.DTOs.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwner _owner;

        public OwnerController(IOwner owner)
        {
            _owner = owner;
        }

        [HttpGet("dashboard/overview")]
        public Task<OwnerOverviewDto> GetOverview([FromQuery] OwnerOverviewQuery query) =>
            _owner.GetOverviewAsync(query);

        [HttpGet("profile")]
        public Task<OwnerProfileDto> GetProfile() =>
            _owner.GetProfileAsync();

        [HttpPut("profile")]
        public Task<OwnerProfileDto> UpdateProfile([FromBody] OwnerProfileUpdateRequest request) =>
            _owner.UpdateProfileAsync(request);

        [HttpPut("profile/password")]
        public Task<OwnerActionResultDto> ChangePassword([FromBody] OwnerChangePasswordRequest request) =>
            _owner.ChangePasswordAsync(request);

        [HttpGet("centers")]
        public Task<IReadOnlyList<OwnerCenterDto>> GetCenters() =>
            _owner.GetCentersAsync();

        [HttpGet("centers/{centerId:int}")]
        public Task<OwnerCenterDto> GetCenter([FromRoute] int centerId) =>
            _owner.GetCenterAsync(centerId);

        [HttpPost("centers")]
        public Task<OwnerCenterDto> CreateCenter([FromBody] OwnerCenterCreateRequest request) =>
            _owner.CreateCenterAsync(request);

        [HttpPut("centers/{centerId:int}")]
        public Task<OwnerCenterDto> UpdateCenter(
            [FromRoute] int centerId,
            [FromBody] OwnerCenterUpdateRequest request) =>
            _owner.UpdateCenterAsync(centerId, request);

        [HttpDelete("centers/{centerId:int}")]
        public Task<OwnerActionResultDto> DeleteCenter([FromRoute] int centerId) =>
            _owner.DeleteCenterAsync(centerId);

        [HttpGet("centers/{centerId:int}/workspace")]
        public Task<OwnerCenterWorkspaceDto> GetCenterWorkspace([FromRoute] int centerId) =>
            _owner.GetCenterWorkspaceAsync(centerId);

        [HttpGet("centers/{centerId:int}/resources")]
        public Task<IReadOnlyList<OwnerResourceDto>> GetResources([FromRoute] int centerId) =>
            _owner.GetResourcesAsync(centerId);

        [HttpPost("centers/{centerId:int}/resources")]
        public Task<OwnerResourceDto> CreateResource(
            [FromRoute] int centerId,
            [FromBody] OwnerResourceCreateRequest request) =>
            _owner.CreateResourceAsync(centerId, request);

        [HttpPut("centers/{centerId:int}/resources/{resourceId:int}")]
        public Task<OwnerResourceDto> UpdateResource(
            [FromRoute] int centerId,
            [FromRoute] int resourceId,
            [FromBody] OwnerResourceUpdateRequest request) =>
            _owner.UpdateResourceAsync(centerId, resourceId, request);

        [HttpDelete("centers/{centerId:int}/resources/{resourceId:int}")]
        public Task<OwnerActionResultDto> DeleteResource(
            [FromRoute] int centerId,
            [FromRoute] int resourceId) =>
            _owner.DeleteResourceAsync(centerId, resourceId);

        [HttpPut("centers/{centerId:int}/resources/{resourceId:int}/units")]
        public Task<OwnerResourceDto> UpdateResourceUnits(
            [FromRoute] int centerId,
            [FromRoute] int resourceId,
            [FromBody] OwnerResourceUnitsUpdateRequest request) =>
            _owner.UpdateResourceUnitsAsync(centerId, resourceId, request);

        [HttpGet("centers/{centerId:int}/services")]
        public Task<IReadOnlyList<OwnerServiceDto>> GetServices([FromRoute] int centerId) =>
            _owner.GetServicesAsync(centerId);

        [HttpPost("centers/{centerId:int}/services")]
        public Task<OwnerServiceDto> CreateService(
            [FromRoute] int centerId,
            [FromBody] OwnerServiceCreateRequest request) =>
            _owner.CreateServiceAsync(centerId, request);

        [HttpPut("centers/{centerId:int}/services/{serviceId:int}")]
        public Task<OwnerServiceDto> UpdateService(
            [FromRoute] int centerId,
            [FromRoute] int serviceId,
            [FromBody] OwnerServiceUpdateRequest request) =>
            _owner.UpdateServiceAsync(centerId, serviceId, request);

        [HttpDelete("centers/{centerId:int}/services/{serviceId:int}")]
        public Task<OwnerActionResultDto> DeleteService(
            [FromRoute] int centerId,
            [FromRoute] int serviceId) =>
            _owner.DeleteServiceAsync(centerId, serviceId);

        [HttpGet("bookings")]
        public Task<IReadOnlyList<OwnerBookingDto>> GetBookings([FromQuery] OwnerBookingsQuery query) =>
            _owner.GetBookingsAsync(query);

        [HttpGet("bookings/{bookingId:int}")]
        public Task<OwnerBookingDto> GetBooking([FromRoute] int bookingId) =>
            _owner.GetBookingAsync(bookingId);

        [HttpPatch("bookings/{bookingId:int}/status")]
        public Task<OwnerBookingDto> UpdateBookingStatus(
            [FromRoute] int bookingId,
            [FromBody] OwnerBookingStatusUpdateRequest request) =>
            _owner.UpdateBookingStatusAsync(bookingId, request);

        [HttpGet("requests")]
        public Task<IReadOnlyList<OwnerCustomerRequestDto>> GetRequests([FromQuery] OwnerRequestsQuery query) =>
            _owner.GetRequestsAsync(query);

        [HttpGet("requests/{requestId:int}")]
        public Task<OwnerCustomerRequestDto> GetRequest([FromRoute] int requestId) =>
            _owner.GetRequestAsync(requestId);

        [HttpPatch("requests/{requestId:int}/status")]
        public Task<OwnerCustomerRequestDto> UpdateRequestStatus(
            [FromRoute] int requestId,
            [FromBody] OwnerRequestStatusUpdateRequest request) =>
            _owner.UpdateRequestStatusAsync(requestId, request);

        [HttpGet("employees")]
        public Task<IReadOnlyList<OwnerEmployeeDto>> GetEmployees([FromQuery] OwnerEmployeesQuery query) =>
            _owner.GetEmployeesAsync(query);

        [HttpPost("employees")]
        public Task<OwnerEmployeeDto> CreateEmployee([FromBody] OwnerEmployeeCreateRequest request) =>
            _owner.CreateEmployeeAsync(request);

        [HttpPut("employees/{employeeId:int}")]
        public Task<OwnerEmployeeDto> UpdateEmployee(
            [FromRoute] int employeeId,
            [FromBody] OwnerEmployeeUpdateRequest request) =>
            _owner.UpdateEmployeeAsync(employeeId, request);

        [HttpDelete("employees/{employeeId:int}")]
        public Task<OwnerActionResultDto> DeleteEmployee([FromRoute] int employeeId) =>
            _owner.DeleteEmployeeAsync(employeeId);
    }
}
```



## Validation, claims, and HTTP responses

Follow the validation and response style in your `AdminController`, but use the actual return types and exception behavior of the existing `OwnerBLL` rather than adding a second result model:

- Keep `[ApiController]` so invalid DTO model state (for example `[Required]`, `[StringLength]`, `[EmailAddress]`, and `[Range]`) automatically returns `400 Bad Request`.
- Explicitly reject a null request body with `BadRequest`, as in the AdminController example. Add data-annotation validation to the existing request DTOs where appropriate; do not make replacement DTOs just to add validation unless changing the shared DTO would break other consumers.
- Keep `[Authorize(Roles = "Owner")]`. Read `ClaimTypes.NameIdentifier`, parse the owner's integer ID, return `401 Unauthorized` if the claim is missing or invalid, and pass the ID to every BLL method. The BLL must scope lookups and mutations to that owner, including center/resource/service IDs from the route.
- The supplied BLL currently returns DTOs/lists, `bool`/nullable `bool`, and throws exceptions; it does not show `OwnerResult<T>`. Reuse those contracts. Map known failures to HTTP status codes at the API boundary or through existing global exception middleware. `ArgumentException`/`ArgumentNullException` generally map to `400`; `KeyNotFoundException` to `404`; `UnauthorizedAccessException` to `403`; unexpected exceptions to `500` (log server-side and return a safe message). `InvalidOperationException` is used for different cases in the snapshot, so map it according to the particular method or refine the existing BLL error contract before mapping; do not guess from message text.
- For methods returning `bool`, treat `true` as the documented success status. Do not automatically treat `false` as `204`; decide whether it means not-found, conflict, or operation failure based on the existing BLL contract, then map consistently. If that distinction is currently unavailable, improve the existing BLL result/exception contract instead of creating parallel business logic.
- Existing create methods such as `AddCenter`, `AddResource`, and `AddService` return `bool`, not the created DTO. A `201 Created` response can be bodyless if that fits the client contract; if the frontend needs the persisted DTO/ID in the response, update the existing BLL method to return or retrieve the existing response DTO rather than adding a duplicate create path.
- Do not put business rules in the controller. The controller checks request/identity and translates the existing BLL outcome; the BLL owns resource validation, ownership checks, and business operations.

### Response status conventions for every endpoint

Add `[ProducesResponseType]` declarations matching the actual action result and BLL contract. These are intended HTTP outcomes; document only outcomes the implementation can really return:

| Endpoint operation | Success response | Other expected responses |
|---|---|---|
| GET overview, profile, centers, center/workspace, resources, services, bookings, requests, employees | `200 OK` with the DTO or list | `401 Unauthorized`, `403 Forbidden`; `404 Not Found` for a requested record/center that is missing or not owned; `500 Internal Server Error` for backend failure |
| POST center, resource, service, employee | `201 Created` with the created DTO | `400 Bad Request` for invalid model/request data, `401 Unauthorized`, `403 Forbidden`, `404 Not Found` for missing or unowned parent center, `409 Conflict` for a duplicate/conflicting record, `500 Internal Server Error` |
| PUT profile, center, resource, service, employee; PATCH booking/request status; PUT resource units | `200 OK` with the updated DTO | `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `409 Conflict` where a uniqueness/state conflict can occur, `500 Internal Server Error` |
| DELETE center, resource, service, employee; PUT profile password | `204 No Content` | `400 Bad Request` where applicable, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `409 Conflict` where deletion is blocked by related data, `500 Internal Server Error` |

`[Authorize]` normally produces `401` for an unauthenticated caller and `403` for an authenticated caller without the Owner role. The claim guard returns `401` if the expected owner ID claim is absent or not an integer. Include those authorization responses in each action's API documentation. The shared claim extraction/exception-to-response handling may live in a base controller or global exception middleware if the backend already uses one; reuse it rather than duplicating it or inventing a new result-wrapper pattern.

## Endpoints and frontend data

### Overview

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("dashboard/overview")]` | `GET /api/Owner/dashboard/overview` | Supplies the owner dashboard in one response: owner display name, center counts/status, booking totals and upcoming/completed/cancelled counts, revenue and rating summaries, active resource/service counts, and chart series for the dashboard's booking/revenue activity. `OwnerOverviewQuery` can contain an optional `from` and `to` date range. |

### My Centers and Center Management

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("centers")]` | `GET /api/Owner/centers` | Lists centers belonging to the authenticated owner. Returns the center cards' identity, location, cover image URL, status, device/resource and service counts, rating, and branch data. |
| `[HttpGet("centers/{centerId:int}")]` | `GET /api/Owner/centers/{centerId}` | Returns the full center details used by the center view and edit form. |
| `[HttpPost("centers")]` | `POST /api/Owner/centers` | Creates a center and returns the saved center, including its generated ID and persisted status. |
| `[HttpPut("centers/{centerId:int}")]` | `PUT /api/Owner/centers/{centerId}` | Updates the center's editable profile and returns the updated center. |
| `[HttpDelete("centers/{centerId:int}")]` | `DELETE /api/Owner/centers/{centerId}` | Removes a center (or applies the backend's configured archive policy) and returns an action result. |

Center data follows the UI fields: `name`, `description`, `country`, `city`, `address`, `coords`, `phone`, `hours`, `centerType`, `devices`, `deviceTypes`, `status`, `rating`, `services`, `branches`, `galleryImages`, and `image`. Store image files and expose URLs or media IDs in the API; do not persist browser data URLs as the image contract.

### Center Workspace

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("centers/{centerId:int}/workspace")]` | `GET /api/Owner/centers/{centerId}/workspace` | Returns the selected center summary used by Workspace Overview: center details, resource-type count, total units, booking count, revenue summary, resource availability counts, and totals for employees/offers/tournaments if available. |
| `[HttpGet("centers/{centerId:int}/resources")]` | `GET /api/Owner/centers/{centerId}/resources` | Lists resource types for the selected center, including category, description, unit, hourly price, status, active flag, image URL, and resource units. |
| `[HttpPost("centers/{centerId:int}/resources")]` | `POST /api/Owner/centers/{centerId}/resources` | Creates a resource type for the center and returns the saved resource. |
| `[HttpPut("centers/{centerId:int}/resources/{resourceId:int}")]` | `PUT /api/Owner/centers/{centerId}/resources/{resourceId}` | Updates the resource's editable details, status, price, image URL, and active state. |
| `[HttpDelete("centers/{centerId:int}/resources/{resourceId:int}")]` | `DELETE /api/Owner/centers/{centerId}/resources/{resourceId}` | Deletes a resource type and returns an action result. |
| `[HttpPut("centers/{centerId:int}/resources/{resourceId:int}/units")]` | `PUT /api/Owner/centers/{centerId}/resources/{resourceId}/units` | Replaces or updates the resource's unit list; each unit has an ID, label, and status (`Available`, `Reserved`, `Maintenance`, or `Disabled`). Returns the updated resource. |
| `[HttpGet("centers/{centerId:int}/services")]` | `GET /api/Owner/centers/{centerId}/services` | Lists services for the center with `name`, `description`, `icon`, optional image URL, and active state. |
| `[HttpPost("centers/{centerId:int}/services")]` | `POST /api/Owner/centers/{centerId}/services` | Creates a center service and returns it. |
| `[HttpPut("centers/{centerId:int}/services/{serviceId:int}")]` | `PUT /api/Owner/centers/{centerId}/services/{serviceId}` | Updates service details or its enabled/disabled state and returns it. |
| `[HttpDelete("centers/{centerId:int}/services/{serviceId:int}")]` | `DELETE /api/Owner/centers/{centerId}/services/{serviceId}` | Deletes a service and returns an action result. |

### Bookings

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("bookings")]` | `GET /api/Owner/bookings` | Returns owner bookings, optionally filtered with `centerId`, `status`, `date`, and `q` (player or device search). The same endpoint serves the owner-wide Bookings page and center-scoped Workspace tab. |
| `[HttpGet("bookings/{bookingId:int}")]` | `GET /api/Owner/bookings/{bookingId}` | Returns one booking for the booking detail dialog, including player, center, device/resource, date, time, duration, total, status, and notes. |
| `[HttpPatch("bookings/{bookingId:int}/status")]` | `PATCH /api/Owner/bookings/{bookingId}/status` | Changes a booking status and returns the updated booking. Status values shown in the UI are `Pending`, `Confirmed`, `Completed`, and `Cancelled`. |

### Customer Requests

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("requests")]` | `GET /api/Owner/requests` | Lists owner customer requests. Optional `type` and `status` filters support the UI filters. Rows include request ID, type, player, subject, message, date, and status. |
| `[HttpGet("requests/{requestId:int}")]` | `GET /api/Owner/requests/{requestId}` | Returns one request for the view dialog. |
| `[HttpPatch("requests/{requestId:int}/status")]` | `PATCH /api/Owner/requests/{requestId}/status` | Accepts or rejects a request and returns its updated status. Values are `Pending`, `Accepted`, and `Rejected`. |

### Employees

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("employees")]` | `GET /api/Owner/employees` | Lists employees; optional `centerId` scopes the response to a center workspace. Returns employee ID, center, name, phone, email, role, permissions, and status. |
| `[HttpPost("employees")]` | `POST /api/Owner/employees` | Creates an employee assigned to a center and returns the saved employee. |
| `[HttpPut("employees/{employeeId:int}")]` | `PUT /api/Owner/employees/{employeeId}` | Updates employee details, role, permissions, assigned center, or status (`Active`, `On leave`, `Suspended`). Returns the updated employee. |
| `[HttpDelete("employees/{employeeId:int}")]` | `DELETE /api/Owner/employees/{employeeId}` | Removes an employee and returns an action result. |

Do not return employee passwords from employee list/detail endpoints. If employee provisioning requires a password, accept it only on the create/provision request and let the auth/business layer handle it.

### Owner Profile

| HTTP attribute | Endpoint | What it does / data returned to the frontend |
|---|---|---|
| `[HttpGet("profile")]` | `GET /api/Owner/profile` | Returns the authenticated owner's `name`, `email`, `phone`, `city`, and `company`. |
| `[HttpPut("profile")]` | `PUT /api/Owner/profile` | Saves the editable owner profile fields and returns the updated profile. |
| `[HttpPut("profile/password")]` | `PUT /api/Owner/profile/password` | Changes the authenticated owner's password using the current and new password. The UI's confirmation password is client-side confirmation and need not be sent to the backend. Returns an action result; never returns password values. |

## DTO reuse and fields required by the frontend

The field lists in the endpoint tables describe what the Next.js screens need, not names of DTO classes that must be added. The supplied BLL already uses these relevant DTOs; reuse them when their fields fit the API response/request:

| Frontend contract | Existing DTO(s) from the supplied BLL snapshot |
|---|---|
| Owner dashboard | `DTO_OwnerDashboard` |
| Owner profile read/update | `DTO_UserInfoRequest`, `DTO_UpdateUserInfoRequest` |
| Center list/create/update | `DTO_CentersListDetails`, `DTO_CreateCenter`, `DTO_UpdateCenter` |
| Resource create/update | `DTO_AddResource`, `DTO_UpdateResource` |
| Service create/update | `DTO_AddService`, `DTO_UpdateService` |
| Booking list/detail/create/update | `DTO_BookingDetails`, `DTO_BookingBase`, `DTO_UpdateBooking` |
| Staff list/create/update/roles | `DTO_StaffDetails`, `DTO_AddStaffMember`, `DTO_UpdateUserInfoRequest`, `DTO_StaffRole`, `DTO_SetRoleToMember` |

Before creating another DTO, inspect these existing definitions and confirm their serialized fields match the response/request needed by the screen. The endpoint contract requires centers to expose fields used by My Centers and Center Workspace (`id`, name, location, status, rating, image, device/service counts and details); resources to expose category, pricing, status, active state and units; services to expose name, description, icon/image and active state; bookings to expose player, center, device/resource, date/time, duration, total, status and notes; employees to expose center, contact details, role, permissions and status; and profile to expose name, email, phone, city and company. If an existing DTO lacks fields, extend it when safe for its other API consumers. Otherwise add a narrowly scoped DTO under the existing `Domain.DTOs` namespace structure and map to/from the existing BLL/domain types—do not duplicate the BLL or persistence behavior.

The supplied excerpt does not show DTOs for the center-workspace aggregate or customer requests. Search the full solution first. If none exists, add only the missing DTO and owner-scoped BLL method; add a DLL/repository operation only if the existing BLL/data layer cannot provide the underlying data. Filter DTOs such as `OwnerBookingsQuery`, `OwnerRequestsQuery`, and `OwnerEmployeesQuery` are also optional contract names, not mandatory new types: use existing query parameters/DTOs where available, and add query DTOs only if the backend's established convention calls for them.

Never return employee passwords, password hashes, or password-change request values. Reuse the existing auth password request and `IAuthService` method for password changes.

## Sections not currently wired to working screens

The global Offers, Tournaments, Revenue, and Reports pages and the corresponding Center Workspace tabs currently render the "Coming soon" state and are disabled in the sidebar/tabs. This specification does not add their future CRUD/report endpoints because there is no active frontend behavior or request/response data contract to map yet. The overview currently displays some mock/derived figures from these areas; populate those aggregate fields from `GET /dashboard/overview` only when real backend data exists.
