using API.Dtos.Auth;
using API.Helper.Params.Auth;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_Auth
    {
        Task<UserForLoggedDTO> Login(UserLoginParam userForLogin);
    }
}