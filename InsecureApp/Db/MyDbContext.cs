using InsecureApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InsecureApp.Db
{
  public class MyDbContext : DbContext
  {
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
  }
}
