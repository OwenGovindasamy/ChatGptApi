using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using ChatGptApi.TokenConfig;
using ChatGptApi.TokenConfig.Models;
using ChatGptApi.Models;

namespace ChatGptApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdministrationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;
        public AdministrationController(
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender, 
            ITokenService tokenService
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailSender = emailSender;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<UserToken> Login(string Email, string Password, bool RememberMe)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Email);

                if (user == null)
                {
                    throw new Exception("Unauthorized");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, Password, false);

                if (result.Succeeded)
                {
                    return new UserToken
                    {
                        Token = _tokenService.CreateToken(user),
                        UserId = user.Id
                    };
                }
                throw new Exception("Username or Password is incorrect.");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            throw new Exception("Invalid credentials.");
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();

            if (returnUrl != null)
            {
                return Ok(returnUrl);
            }
            return BadRequest("Something went wrong, Please try again.");
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<ActionResult> CreateUser(SignUp model)
        {
            List<string> errors = new List<string>();

            if (ModelState.IsValid)
            {
                var user = CreateUserInternal();
                user.PhoneNumber = model.PhoneNumber;

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Administration/",
                        pageHandler: null,
                        values: new { Action = "ConfirmEmail", userId, code },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return BadRequest("Please confirm your account by clicking the link sent to you on email.");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        var data = new UserToken
                        {
                            Token = _tokenService.CreateToken(user),
                            UserId = user.Id
                        };
                        return Ok(data);
                    }
                }
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
            }

            return BadRequest(errors);
        }

        private IdentityUser CreateUserInternal()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

    }
}
