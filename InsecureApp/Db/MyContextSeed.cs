using InsecureApp.Entities;

namespace InsecureApp.Db
{
  public static class MyDbContextSeed
  {
    public static void SeedData(MyDbContext context)
    {
      if (!context.Users.Any())
      {
        context.Users.AddRange(
            new User { Name = "admin", Password = "password" },
            new User { Name = "user1", Password = "password123" }
        );

        context.SaveChanges();
      }
    }
  }
}
