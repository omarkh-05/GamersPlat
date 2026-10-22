using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_BookingBase
    {
        public int CenterId { get; set; }
        public int ResourcesTypeId { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }

    // الحالة 1: يوزر مسجل دخول (لا اسم ولا رقم ولا userId من الكلاينت)
    public class DTO_AddBooking : DTO_BookingBase { }

    // الحالة 2: ضيف بدون حساب
    public class DTO_AddGuestBooking : DTO_BookingBase
    {
        [Required, MaxLength(100)] public string CustomerName { get; set; } = null!;
        [Required, MaxLength(20)] public string PhoneNumber { get; set; } = null!;
        [Required, MinLength(8)] public string Password { get; set; } = null!;
    }

    // الحالة 3: أدمن/موظف
    public class DTO_AdminAddBooking : DTO_BookingBase
    {
        [MaxLength(100)] public string? CustomerName { get; set; }
        [MaxLength(20)] public string? PhoneNumber { get; set; }
    }
}
