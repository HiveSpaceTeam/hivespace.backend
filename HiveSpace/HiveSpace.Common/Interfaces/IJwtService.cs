using HiveSpace.Common.Models;

namespace HiveSpace.Common.Interface;

public interface IJwtService
{
    string GenerateToken(Identity user);
}
