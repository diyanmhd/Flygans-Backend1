using Flygans_Backend.DTOs.Users;
using Flygans_Backend.Helpers;
using Flygans_Backend.Repositories.Users;
using Flygans_Backend.Exceptions; // required

namespace Flygans_Backend.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IAdminUserRepository _userRepo;

        public UserService(IAdminUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ServiceResponse<List<UserResponseDto>>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllUsersAsync();

            return new ServiceResponse<List<UserResponseDto>>
            {
                Success = true,
                Data = users.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsBlocked = u.IsBlocked
                }).ToList()
            };
        }

        public async Task<ServiceResponse<bool>> BlockUserAsync(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null || user.IsDeleted)
                throw new NotFoundException("User not found");

            var success = await _userRepo.BlockUserAsync(user);

            if (!success)
                throw new NotFoundException("User not found");

            return new ServiceResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "User blocked"
            };
        }

        public async Task<ServiceResponse<bool>> UnblockUserAsync(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null || user.IsDeleted)
                throw new NotFoundException("User not found");

            var success = await _userRepo.UnblockUserAsync(user);

            if (!success)
                throw new NotFoundException("User not found");

            return new ServiceResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "User unblocked"
            };
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            var success = await _userRepo.DeleteUserAsync(user);

            if (!success)
                throw new NotFoundException("User not found");

            return new ServiceResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "User deleted"
            };
        }
    }
}
