using Domain.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bussiness.Helpers
{
    public class ValidateRegisterDto
    {

        public static string? Validate(DTO_AddGuestBooking dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.CustomerName) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return "credentials required";

            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.CustomerName = dto.CustomerName.Trim();

            if (!JordanPhoneRegex.IsMatch(dto.PhoneNumber))
                return "invalid phone number";

            if (dto.CustomerName.Length < 2 || dto.CustomerName.Length > 100)
                return "invalid name";

            if (dto.Password.Length < 8 || dto.Password.Length > 100)
                return "password must be at least 8 characters";

            return null;
        }
        private static readonly Regex JordanPhoneRegex = new(@"^07[789]\d{7}$", RegexOptions.Compiled);
    }
}
