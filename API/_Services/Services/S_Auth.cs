using API._Repositories;
using API._Services.Interfaces;
using API.Dtos.Auth;
using API.Helper.Params.Auth;

namespace API._Services.Services
{
    public class S_Auth : I_Auth
    {
        private readonly IRepositoryAccessor _repoAccessor;
        public S_Auth(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }
        public async Task<UserForLoggedDTO> Login(UserLoginParam userForLogin)
        {
            // Kiểm tra sự tồn tại của user
            var user = await _repoAccessor.NguoiDung.FindSingle(x => x.TaiKhoan == userForLogin.Username && x.MatKhau == userForLogin.Password);
            if (user == null)
                return null;

            var result = new UserForLoggedDTO
            {
                Name = user.HoTen
            };
            return result;
        }
    }
}