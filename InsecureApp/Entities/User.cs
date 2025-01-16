namespace InsecureApp.Entities
{
  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }  // Bu şifre, hashlenmeden kaydedilecek
  }
}
