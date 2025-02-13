using System.ComponentModel.DataAnnotations;

namespace Authentication__ASP.Net_Core__.Entities.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string FirtstName { get; set; }
        [Required]
        public required string MiddleName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiresAt { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiryTime { get; set; }

        public string PhoneNumber { get; set; }
        public string? OTPToken { get; set; }
        public bool IsOTPVerified { get; set; } = false;
        public DateTime? OTPVerificationExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // foreign key
        public int RoleId { get; set; }
        public Role Role { get; set; }


        //nvaigation property
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
