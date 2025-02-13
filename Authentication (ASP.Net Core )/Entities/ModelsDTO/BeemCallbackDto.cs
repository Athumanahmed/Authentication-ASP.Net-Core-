namespace Authentication__ASP.Net_Core__.Entities.ModelsDTO
{
    public class BeemCallbackDto
    {
        public string MessageId { get; set; }
        public string Recipient { get; set; }
        public string Status { get; set; }  // "DELIVERED", "FAILED", "PENDING"
        public DateTime Timestamp { get; set; }
    }
}
