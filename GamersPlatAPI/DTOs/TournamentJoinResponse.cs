namespace GamersPlatAPI.DTOs
{
    public class TournamentJoinResponse
    {
        public int TournamentId { get; set; }
        public string Message { get; set; } = null!;
        public int ParticipantsCount { get; set; }
    }
}
