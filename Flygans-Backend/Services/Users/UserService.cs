using Flygans_Backend.DTOs.Users;
using Flygans_Backend.Helpers;
using Flygans_Backend.Repositories.Users;

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
            var res = new ServiceResponse<bool>();

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                res.Success = false;
                res.Message = "User not found";
                return res;
            }

            var result = await _userRepo.BlockUserAsync(user);

            if (!result)
            {
                res.Success = false;
                res.Message = "User not found";
                return res;
            }

            res.Success = true;
            res.Data = true;
            res.Message = "User blocked";
            return res;
        }

        public async Task<ServiceResponse<bool>> UnblockUserAsync(int userId)
        {
            var res = new ServiceResponse<bool>();

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                res.Success = false;
                res.Message = "User not found";
                return res;
            }

            var result = await _userRepo.UnblockUserAsync(user);

            if (!result)
            {
                res.Success = false;
                res.Message = "User not found";
                return res;
            }

            res.Success = true;
            res.Data = true;
            res.Message = "User unblocked";
            return res;
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(int userId)
        {
            var res = new ServiceResponse<bool>();

            var user = await _userRepo.GetUserByIdAsync(userId);

            // user doesn't exist
            if (user == null)
            {
                res.Success = false;
                res.Message = "User not found";
                return res;
            }

            // delegate soft delete to repo
            var result = await _userRepo.DeleteUserAsync(user);

            // already deleted or failed
            if (!result)
            {
                res.Success = false;
                res.Message = "User not found";
                return res;
            }

            // success
            res.Success = true;
            res.Data = true;
            res.Message = "User deleted";
            return res;
        }
    }
}
