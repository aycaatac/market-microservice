namespace MailService.API.Message
{
    public class RewardMessage
    {
        public string UserId { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
        public string? Email { get; set; }
        public double? OrderTotal { get; set; }
        public DateTime? OrderTime { get; set; }
        public string? EmailMessage { get; set; }
    }
}
