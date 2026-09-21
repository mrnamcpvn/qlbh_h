using API._Repositories;
using API._Services.Interfaces;
using API.Data;
using API.Dtos.Auth;
using API.Helper.Params.Auth;

namespace API._Services.Services
{
    public class S_Auth : BaseServices, I_Auth
    {
        public S_Auth(DBContext dbContext) : base(dbContext) { }
        public async Task<UserForLoggedDTO> Login(UserLoginParam userForLogin)
        {
            // Kiểm tra sự tồn tại của user
            var user = await _repoAccessor.NguoiDung.FirstOrDefaultAsync(x => x.TaiKhoan == userForLogin.Username && x.MatKhau == userForLogin.Password);
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