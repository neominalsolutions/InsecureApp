using System.ComponentModel;

namespace InsecureApp.Models
{
  public class LoginModel
  {
    [DefaultValue("admin")]
    public string Username { get; set; }

    [DefaultValue("password")]
    public string Password { get; set; }
  }
}
