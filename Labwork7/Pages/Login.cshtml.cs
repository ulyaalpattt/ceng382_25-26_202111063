using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Labwork5.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Username == "admin" && Password == "1234")
            {
                var sessionId = Guid.NewGuid().ToString();

                // Session ayarları
                HttpContext.Session.SetString("username", Username);
                HttpContext.Session.SetString("token", "abc123");
                HttpContext.Session.SetString("session_id", sessionId);

                // Cookie ayarları
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),  // Çerezin geçerlilik süresi
                    HttpOnly = true,  // Sadece HTTP istekleriyle erişilebilir
                    Secure = true,  // Sadece HTTPS üzerinden gönderilebilir
                    SameSite = SameSiteMode.Strict  // Çerez sadece aynı site üzerinde gönderilebilir
                };

                // Çerezlerin ayarlanması
                Response.Cookies.Append("username", Username, cookieOptions);
                Response.Cookies.Append("token", "abc123", cookieOptions);
                Response.Cookies.Append("session_id", sessionId, cookieOptions);

                return RedirectToPage("/Index");
            }

            TempData["Error"] = "Invalid login.";
            return Page();
        }
    }
}
