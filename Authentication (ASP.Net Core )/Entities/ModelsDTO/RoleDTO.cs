namespace Authentication__ASP.Net_Core__.Entities.ModelsDTO
{
    public class RoleDTO
    {
        public string RoleName { get; set; }
    }

    public class AssignUserRoleDTO
    {
        public Guid UserId { get; set; } 
        public int RoleId { get; set; }  
    }

    public class AllRoleDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }


}
