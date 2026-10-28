using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Center;
using Domain.DTOs.User;

namespace Domain.DTOs.Owner;

public sealed class DTO_OwnerOverviewQuery
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
}

public sealed class DTO_OwnerActivityPoint
{
    public DateOnly Date { get; set; }
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
}

public sealed class DTO_OwnerWorkspace
{
    public DTO_CenterDetails Center { get; set; } = new();
    public int ResourceTypeCount { get; set; }
    public int TotalUnits { get; set; }
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
    public int AvailableUnits { get; set; }
    public int ReservedUnits { get; set; }
    public int MaintenanceUnits { get; set; }
    public int DisabledUnits { get; set; }
    public int EmployeeCount { get; set; }
    public int OfferCount { get; set; }
    public int TournamentCount { get; set; }
}

public sealed class DTO_OwnerResourceUnit
{
    public int ResourceUnitId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
}

public sealed class DTO_OwnerResource
{
    public int ResourcesTypeId { get; set; }
    public int CenterId { get; set; }
    public byte DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? RoomType { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal HourlyPrice { get; set; }
    public int TotalQuantity { get; set; }
    public bool IsActive { get; set; }
    public List<DTO_OwnerResourceUnit> Units { get; set; } = [];
}

public sealed class DTO_OwnerResourceUnitsUpdate
{
    [Required]
    public List<DTO_OwnerResourceUnitUpdate> Units { get; set; } = [];
}

public sealed class DTO_OwnerResourceUnitUpdate
{
    public int? ResourceUnitId { get; set; }

    [Required, StringLength(100)]
    public string Label { get; set; } = string.Empty;

    [Required, RegularExpression("^(Available|Reserved|Maintenance|Disabled)$")]
    public string Status { get; set; } = "Available";
}

public sealed class DTO_OwnerService
{
    public byte ServiceId { get; set; }
    public int CenterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public sealed class DTO_OwnerBookingsQuery
{
    public int? CenterId { get; set; }
    public string? Status { get; set; }
    public DateOnly? Date { get; set; }
    public string? Q { get; set; }
}

public sealed class DTO_OwnerBookingStatusUpdate
{
    [Required]
    [RegularExpression("^(?i:Pending|Confirmed|Completed|Cancelled)$")]
    public string Status { get; set; } = string.Empty;
}

public sealed class DTO_OwnerCustomerRequest
{
    public int RequestId { get; set; }
    public int CenterId { get; set; }
    public int? PlayerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class DTO_OwnerRequestStatusUpdate
{
    [Required]
    [RegularExpression("^(?i:Accepted|Rejected)$")]
    public string Status { get; set; } = string.Empty;
}

public sealed class DTO_OwnerEmployeesQuery
{
    public int? CenterId { get; set; }
}

public sealed class DTO_OwnerEmployee
{
    public int StaffId { get; set; }
    public int UserId { get; set; }
    public int CenterId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string CenterName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Permissions { get; set; }
    public bool IsActive { get; set; }
}

public sealed class DTO_OwnerEmployeeCreateRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 7)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [EmailAddress, StringLength(255)]
    public string? Email { get; set; }

    [Range(1, short.MaxValue)]
    public short CityId { get; set; }

    [Range(1, int.MaxValue)]
    public int CenterId { get; set; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string Role { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Permissions { get; set; }
}

public sealed class DTO_OwnerEmployeeUpdateRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 7)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress, StringLength(255)]
    public string? Email { get; set; }

    [Range(1, short.MaxValue)]
    public short CityId { get; set; }

    [Range(1, int.MaxValue)]
    public int CenterId { get; set; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string Role { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Permissions { get; set; }

    public bool IsActive { get; set; }
}

