using AspNetCoreWithAngular.Data.Entities;
using AspNetCoreWithAngular.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration config)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Der SignInManager sorgt dafür, dass der aktuelle User angemeldet wird
                var result = await _signInManager.PasswordSignInAsync(
                        model.Username,
                        model.Password,
                        model.RememberMe,
                        false);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        //Wenn eine ReturnUrl angegeben wurde, wird diese aufgerufen
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        //Ansonsten gehts zum Shop
                        //Es kann überall hingehen - hier ist es der Shop
                        return RedirectToAction("Shop", "Home");
                    }
                }
            }
            else
            {

            }

            //Falls der User nicht angemeldet werden konnte,
            //bleibt das Login-Page sichtbar
            ModelState.AddModelError("", "Failed to login");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Erstellt einen Token für einen bestimmten User (model)
        //Kann über Postman getestet werden
        //Aufruf: http://localhost/account/CreateToken
        //        Dabei muss im Body Username und Password eingegeben werden
        //        { "username": "xxyyzz", "bboerding@web.de": "P@ssw0rd!" }
        //Daraufhin wird ein Token für den User zurückgeschickt
        //Dieser Token kann bei allen weiteren Get-Requests übertragen werden.
        //Dabei muss vorher im Header ein Key-Value-Pair eingetragen werden
        //   Key: Authorization; Value: Bearer "und der token, der vorher erzeugt wurde"
        //Siehe dazu https://app.pluralsight.com/player?course=aspnetcore-mvc-efcore-bootstrap-angular-web Kapitel 9
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userObj = await _userManager.FindByNameAsync(model.Username);
                User user = (User)userObj;

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString())
                        };

                        var keyString = _config["Tokens:Key"];
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
                        var issuer = _config["Tokens:Issuer"];
                        var audience = _config["Tokens:Audience"];
                        var credendials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var expires = DateTime.UtcNow.AddHours(24);
                        var token = new JwtSecurityToken(
                            issuer: issuer,
                            audience: audience,
                            claims: claims,
                            expires: expires,
                            signingCredentials: credendials
                        );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created("", results);
                    }
                }
            }
            return BadRequest();
        }
    }
}
