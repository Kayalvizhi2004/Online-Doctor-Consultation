using ConsultationApi.Domain.Entities.Users;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        string email);

    Task<User?> GetByIdAsync(
        Guid id);

    Task AddAsync(
        User user);

    Task AddRefreshTokenAsync(
        ConsultationApi.Domain.Entities.Users.RefreshToken token);

    Task<ConsultationApi.Domain.Entities.Users.RefreshToken?>
        GetRefreshTokenAsync(string token);

    Task SaveChangesAsync();
}