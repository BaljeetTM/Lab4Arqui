using Shortly.Application.DTOs;

namespace Shortly.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> Register(string email, string password);

    Task<List<UserResponse>> GetAllUsers();

    Task<UserResponse> Login(string email, string password);

    Task<UserResponse> GetUserByEmail(string email);
}
