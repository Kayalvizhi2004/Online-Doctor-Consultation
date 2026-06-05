using ConsultationApi.Domain.Entities.Users;

namespace ConsultationApi.Domain.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}