using Bussiness.Interfaces;
using Data.DLL;

namespace GamersPlatAPI.BackgroundServices
{
    public class BookingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();


                    // 1. انتهاء مدة الحجوزات التي بدأت فعلياً
                    var expiredBookings = await bookingService.GetExpiredRunningBookingIds();
                    if (expiredBookings != null && expiredBookings.Any())
                    {
                        foreach (var bookingId in expiredBookings)
                        {
                            var claimed =
                                await bookingService.TryMarkTimeExpiredNotified(bookingId);

                            if (!claimed)
                                continue;

                            await notificationService.NotifyBookingTimeExpiredAsync(bookingId);
                        }
                    }


                    // 2. إلغاء الحجوزات التي لم تبدأ خلال 15 دقيقة
                    var bookingsReadyForCancellation = await bookingService.GetBookingsReadyForAutoCancellation();
                    if (bookingsReadyForCancellation != null &&
                        bookingsReadyForCancellation.Any())
                    {
                        foreach (var bookingId in bookingsReadyForCancellation)
                        {
                            var cancelled =
                                await bookingService.CancelUnstartedBooking(bookingId);

                            if (!cancelled)
                                continue;

                            await notificationService.NotifyBookingAutoCancelledAsync(bookingId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventLog_Helper.WriteEventLog("Error while checking booking expiration and cancellation Error: ", ex);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
