using UserManagementService.Models;

namespace UserManagementService.Data;

public interface IRepository
{
    bool SaveChanges();

    //User
    void CreateUser(User user);
    bool UserExists(int id);
    bool ExternalUserExists(int externalId);
    Task<User?> GetUserByExternalId(int externalId);
    Task<User?> GetUserById(int id);
    Task<IEnumerable<User>> GetUsers();

    //UserProfile
    Task<UserProfile?> GetUserProfileById(int id);
    bool UserProfileExistByUserId(int id);
    Task<IEnumerable<UserProfile>> GetUserProfiles();
    void CreateUserProfile(UserProfile userProfile);
    void UpdateUserProfile(UserProfile userProfile);
    void UpdateTransferredUserProfileStatus(int id, bool isTransferred);
}