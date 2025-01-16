using InsecureApp.Db;
using InsecureApp.Entities;

namespace InsecureApp.Services
{
  public class UserService : IUserService
  {
    private readonly MyDbContext _context;

    public UserService(MyDbContext context)
    {
      _context = context;
    }

    public User ValidateUser(string username, string password)
    {
      // Güvensiz şifre doğrulama: Şifre düz metin olarak saklanıyor
      return _context.Users.FirstOrDefault(u => u.Name == username && u.Password == password);
    }
  }
}
