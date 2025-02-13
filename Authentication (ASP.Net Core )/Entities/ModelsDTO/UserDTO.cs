namespace Authentication__ASP.Net_Core__.Entities.ModelsDTO
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OTPVerificationRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;  
        public string OTPToken { get; set; } = string.Empty; 
    }

}
