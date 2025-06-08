using AuthService.Dtos;
using AuthService.Models;

namespace AuthService.Data;

public interface IRepository
{
    bool SaveChanges();
    IEnumerable<string> GetRoles();
    Task<IEnumerable<User>> GetUsers();
    Task<User?> FindUserByUserName(string userName);
    Task<IEnumerable<User>> GetNotTransferredUsers();
    void UpdateTransferredUserStatus(int userId, bool transferred);
    void CreateUser(User user);
    void UpdatePassword(User user);
}