using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Users;

public class UserRepository
    : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email);
    }

    public async Task<User?> GetByIdAsync(
        Guid id)
    {
        return await _context.Users
            .Include(x => x.DoctorProfile)
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    public async Task AddAsync(
        User user)
    {
        await _context.Users.AddAsync(
            user);
    }

    public async Task AddRefreshTokenAsync(
        ConsultationApi.Domain.Entities.Users.RefreshToken token)
    {
        await _context.Set<ConsultationApi.Domain.Entities.Users.RefreshToken>()
            .AddAsync(token);
    }

    public async Task<ConsultationApi.Domain.Entities.Users.RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.Set<ConsultationApi.Domain.Entities.Users.RefreshToken>()
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}