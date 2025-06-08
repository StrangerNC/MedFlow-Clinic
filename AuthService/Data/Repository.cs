using AuthService.Dtos;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class Repository(AppDbContext context) : IRepository
{
    public bool SaveChanges()
    {
        return context.SaveChanges() > 0;
    }

    public IEnumerable<string> GetRoles()
    {
        return Enum.GetValues(typeof(Roles)).Cast<Roles>().Select(p => p.ToString());
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await context.Users.Where(x => x.IsAdmin == false).ToListAsync();
    }

    public async Task<User?> FindUserByUserName(string userName)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            context.SaveChanges();
        }

        return user;
    }

    public async Task<IEnumerable<User>> GetNotTransferredUsers()
    {
        return await context.Users.Where(x => !x.IsTransferred && x.IsAdmin == false).ToListAsync();
    }

    public void UpdateTransferredUserStatus(int userId, bool transferred)
    {
        var user = context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null) user.IsTransferred = transferred;
    }

    public void CreateUser(User user)
    {
        context.Users.Add(user);
    }

    public void UpdatePassword(User user)
    {
        context.Users.Update(user);
    }
}