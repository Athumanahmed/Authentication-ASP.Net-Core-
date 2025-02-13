namespace Authentication__ASP.Net_Core__.Entities.ModelsDTO
{
    public class ResPermissionDTO
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public string RoleName { get; set; }  // Role to which this permission is assigned
        public string UserName { get; set; }  // User who has this permission (if applicable)
    }


    public class UpdatePermDTO
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
    }

    public class ReqPermissionDTO
    {
        public string PermissionName { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public int RoleId { get; set; }
    }

    public class AssignPermissionToUserDTO
    {
        public Guid UserId { get; set; }
        public int PermissionId { get; set; }
        public int RoleId { get; set; }  // Role under which this permission is granted
    }
}
