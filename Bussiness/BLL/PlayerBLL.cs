using Data;
using Data.DLL;
using DataLayer;
using Domain.DTOs.Player;

namespace Bussiness.BLL
{
    public class PlayerBLL
    {
        private readonly BookingBLL _booking;
        private readonly ReviewBLL _review;
        private readonly TournamentPlayerBLL _tournament;

        public PlayerBLL(BookingBLL booking, ReviewBLL review, TournamentPlayerBLL tournament)
        {
            _booking = booking;
            _review = review;
            _tournament = tournament;
        }

        // ================ Read Player Profile ================
        public async Task<DTO_PlayerProfile?> GetPlayerProfile(int userId)
        {
                var userInfo = await UserDLL.GetUserInfoByID(userId);

                if (userInfo == null)
                throw new Exception("User not found.");

            var statistics = await PlayerDLL.GetPlayerProfile(userId);

                if (statistics == null)
                throw new Exception("Player Profile not found.");

            return new DTO_PlayerProfile
                {
                    // User Information
                    PhoneNumber = userInfo.PhoneNumber,
                    FullName = userInfo.FullName,
                    Email = userInfo.Email,
                    CityId = userInfo.CityId,
                    Points = userInfo.Points,
                    IsActive = userInfo.IsActive,
                    PhoneVerified = userInfo.PhoneVerified,
                    EmailVerified = userInfo.EmailVerified,

                    // Player Statistics
                    TotalBookings = statistics.TotalBookings,
                    TotalTournaments = statistics.TotalTournaments,
                    TotalPaid = statistics.TotalPaid,
                    MostPlayedCenterId = statistics.MostPlayedCenterId,
                    MostPlayedCenterName = statistics.MostPlayedCenterName
                };
        }
        public async Task<List<DTO_PlayerBookingsInfo>> GetPlayerBooking(int playerId)
        {
            var List = new List<DTO_PlayerBookingsInfo>();
            
            List = await _booking.GetByUserId(playerId);
            if (List == null)
                throw new Exception("No bookings found for the specified user.");
            return List;
        }
        /*
        public async Task<List<DTO_PlayerTournamentInfo>?> GetPlayerTournaments(int playerId)
        {
            var List = new List<DTO_PlayerTournamentInfo>();
            List = await _tournament.GetByPlayerID(playerId);
            if (List == null) throw new Exception("No tournaments found for the specified user.");
            return List;
        }
        
        public async Task<List<DTO_PlayerTournamentInfo>?> GetPlayerSessions(int playerId)
        {
            var List = new List<DTO_PlayerTournamentInfo>();
            List = await _tournament.GetByPlayerID(playerId);
            if (List == null) throw new Exception("No tournaments found for the specified user.");
            return List;
        }
        public async Task<List<Review>?> GetPlayerReviews(int playerId)
        {
            var List = new List<Review>();
            List = await _review.GetByUserId(playerId);
            if (List == null)
                throw new Exception("No reviews found for the specified user.");
            return List;
        }
        */
        // ================ Read Player Profile ================


        // ================ Player Booking Management ================
        public async Task<bool> AddBooking(Booking booking)
        {
            return await _booking.Add(booking) == true;
        }
        public async Task<bool> UpdateBooking(Booking booking)
        {
            return await _booking.Update(booking) == true;
        }
        public async Task<bool> CancelBooking(int bookingID)
        {
            return await _booking.Cancel(bookingID) == true;
        }
        // ================ Player Booking Management ================

        /*
        // ================ Player Review Management ================
        public async Task<bool> AddReview(Review review)
        {
            return await _review.Add(review) == true;
        }
        // ================ Player Review Management ================


        // ================ Player Tournament Management ================
        public async Task<bool> JoinTournament(TournamentPlayer tournamentPlayer)
        {
            return await _tournament.Add(tournamentPlayer) == true;
        }
        public async Task<bool> LeaveTournament(int tournamentPlayerId)
        {
            return await _tournament.Leave(tournamentPlayerId) == true;
        }
        // ================ Player Tournament Management ================


        // ================ Player Session Management ================
        public async Task<bool> CreateSession(Session session)
        {
            return await _session.Add(session) == true;
        }
        public async Task<bool> JoinSession(int sessionId)
        {
            return await _session.Join(sessionId) == true;
        }
        public async Task<bool> LeaveSession(int sessionId)
        {
            return await _session.Leave(sessionId) == true;
        }
        // ================ Player Session Management ================
        */
    }
}
