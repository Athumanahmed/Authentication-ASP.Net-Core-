using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Data;
using Authentication__ASP.Net_Core__.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication__ASP.Net_Core__.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<User>> AddAsync(User user)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (existingUser)
            {
                return new ApiResponse<User>(false, "Email already exists", null, 409);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new ApiResponse<User>(true, "User registered successfully", user, 201);
        }

        public async Task<ApiResponse<User>> AddUpdateUser(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if(existingUser == null)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return new ApiResponse<User>(true, "User created successfully", user, 201);
            }
            else
            {
                existingUser.FirtstName = user.FirtstName;
                existingUser.MiddleName = user.MiddleName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new ApiResponse<User>(true, "User updated successfully", existingUser, 200);
        }

        public async Task<ApiResponse<string>> DeleteAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<string>(false, "User not found", null, 404);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new ApiResponse<string>(true, "User deleted successfully", null, 200);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApiResponse<User>> GetUserByEmailAsync(string email)
        {
          var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if(user == null)
            {
                return new ApiResponse<User>(false, "User not found", null, 404);
            }

            return new ApiResponse<User>(true, "User retrieved successfully", user, 200);
        }

        public async Task<User?> GetUserByTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

        }
        public async Task<ApiResponse<User>> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return new ApiResponse<User>(false, "User not found", null, 404);
            }

            existingUser.FirtstName = user.FirtstName;
            existingUser.MiddleName = user.MiddleName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UpdatedAt = DateTime.UtcNow;


            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return new ApiResponse<User>(true, "User updated successfully", existingUser, 200);
        }
    }
}
