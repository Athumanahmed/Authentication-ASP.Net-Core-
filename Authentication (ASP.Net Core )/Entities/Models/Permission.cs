using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Authentication__ASP.Net_Core__.Entities.Models
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }

        [Required]
        public string PermissionName { get; set; }

        public bool CanRead { get; set; } = false;
        public bool CanWrite { get; set; } = false;

        // Foreign Keys
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
