using Microsoft.EntityFrameworkCore;
using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories;
public class UserRepository(HiveSpaceDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public async Task<User?> FindUserByPhoneNumber(string phoneNumber)
    {
        return await _context.User.Where(x => x.PhoneNumber.Value == phoneNumber).FirstOrDefaultAsync();
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await _context.User.Where(x => x.Email == email).FirstOrDefaultAsync();
    }

    protected override IQueryable<User> ApplyIncludeDetail(IQueryable<User> query)
    {
        return query.Include(x => x.Addresses);
    }
}
