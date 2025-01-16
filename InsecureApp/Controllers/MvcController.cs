
using InsecureApp.Models;
using InsecureApp.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace InsecureApp.Controllers
{
 
  public class MvcController : Controller
  {
    private readonly IUserService _userService;

    public MvcController(IUserService userService)
    {
      _userService = userService;
    }

    // XSS açığı var çünkü inputları doğrulamadan ekrana yazdırıyoruz
    // script=<script>alert(localstorage.getItem('token'))</script>
    
    [HttpGet]
    public IActionResult Index(string script)
    {

      return Content(script, "text/html");  // XSS açığı burada
    }

    [HttpGet]
    public IActionResult Login()
    {
      return View();
    }

    // CSRF açığı var çünkü AntiForgeryToken kontrolü yapılmıyor
    [HttpPost,IgnoreAntiforgeryToken]
    
    public IActionResult Login(LoginModel model)
    {
      var user = _userService.ValidateUser(model.Username, model.Password);
      if (user != null)
      {
        // Güvensiz bir cookie oluşturuluyor
        // Cookie'yi basit bir şekilde kullanıcı adı ve şifreyi düz metin olarak saklıyoruz
        var cookieOptions = new CookieOptions
        {
          Expires = DateTime.Now.AddMinutes(30), // 30 dakika geçerli
          IsEssential = true // Bu cookie, temel işlevsellik için gerekli olduğu belirtilmiş
        };
        // Kullanıcı adı ve şifreyi düz metin olarak saklıyoruz (Bu güvenli değildir!)
        Response.Cookies.Append("UserAuth", $"{model.Username}:{model.Password}", cookieOptions);

        return Redirect("/");
      }
      return View();
    }
  }
}
