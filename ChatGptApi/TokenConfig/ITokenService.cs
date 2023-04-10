using Microsoft.AspNetCore.Identity;

namespace ChatGptApi.TokenConfig
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user);
    }
}
