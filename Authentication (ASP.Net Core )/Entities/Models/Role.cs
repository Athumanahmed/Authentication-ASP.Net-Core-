using System.ComponentModel.DataAnnotations;

namespace Authentication__ASP.Net_Core__.Entities.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }


        // navigation property to users
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
