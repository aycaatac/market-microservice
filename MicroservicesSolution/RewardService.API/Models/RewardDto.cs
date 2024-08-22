namespace RewardService.API.Models
{
	public class RewardDto
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public DateTime RewardsDate { get; set; }
		public int RewardsActivity { get; set; }
		public int OrderId { get; set; }
        public string? Email { get; set; }
        public double? OrderTotal { get; set; }
        public DateTime? OrderTime { get; set; }
        public string? EmailMessage { get; set; }
    }
}
