using Microsoft.EntityFrameworkCore;
using UserManagementService.Models;

namespace UserManagementService.Data;

public class Repository(AppDbContext context) : IRepository
{
    public bool SaveChanges()
    {
        return context.SaveChanges() > 0;
    }

    public void CreateUser(User user)
    {
        context.Users.Add(user);
    }

    public bool UserExists(int id)
    {
        return context.Users.Any(x => x.Id == id);
    }

    public bool ExternalUserExists(int externalId)
    {
        return context.Users.Any(x => x.ExternalId == externalId);
    }

    public async Task<User?> GetUserByExternalId(int externalId)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UserProfile?> GetUserProfileById(int id)
    {
        return await context.UserProfiles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await context.Users.ToListAsync();
    }

    public bool UserProfileExistByUserId(int id)
    {
        var userProfile = context.UserProfiles.FirstOrDefault(x => x.UserId == id);
        return userProfile != null;
    }

    public async Task<IEnumerable<UserProfile>> GetUserProfiles()
    {
        return await context.UserProfiles.ToListAsync();
    }

    public void CreateUserProfile(UserProfile userProfile)
    {
        context.UserProfiles.Add(userProfile);
    }

    public void UpdateUserProfile(UserProfile userProfile)
    {
        context.UserProfiles.Update(userProfile);
    }

    public void UpdateTransferredUserProfileStatus(int id, bool isTransferred)
    {
        var userProfile = context.UserProfiles.FirstOrDefault(x => x.Id == id);
        if (userProfile != null)
            userProfile.IsTransferred = isTransferred;
    }
}