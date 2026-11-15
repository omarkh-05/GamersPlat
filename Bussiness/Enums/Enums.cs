using System;
using System.Collections.Generic;
using System.Text;

namespace Bussiness.Enums
{
    [Flags]
    public enum Permissions : short
    {
        None = 0,
        View = 1,
        AcceptRejectBooking = 2,
        Edit = 4,
        Add = 8,
        Delete = 16
    }
}
