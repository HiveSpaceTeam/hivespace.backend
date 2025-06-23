using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Seedwork;

namespace HiveSpace.Domain.Repositories;
public interface IUserRepository : IRepository<User>
{
    Task<User?> FindUserByPhoneNumber(string phoneNumber);
    Task<User?> FindUserByEmail(string email);
}
