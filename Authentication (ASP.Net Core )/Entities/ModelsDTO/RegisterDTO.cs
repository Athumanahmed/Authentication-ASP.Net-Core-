namespace Authentication__ASP.Net_Core__.Entities.ModelsDTO
{
    public class RegisterDTO
    {
        public required string FirstName { get; set; }
        public required string MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
