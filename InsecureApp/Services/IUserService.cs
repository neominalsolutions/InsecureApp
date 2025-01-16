using InsecureApp.Entities;

namespace InsecureApp.Services
{
  public interface IUserService
  {
    User ValidateUser(string username, string password);
  }
}
