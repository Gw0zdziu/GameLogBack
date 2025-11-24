using GameLogBack.Dtos;
using GameLogBack.Dtos.User;

namespace GameLogBack.Interfaces;

public interface IUserService
{
    public Task<string> RegisterUser(RegisterNewUserDto registerNewUser);
    public Task UpdateUser(UpdateUserDto updateUserDto, string userId);
    public Task<GetUserDto> GetUser(string userId);
    public Task ResendNewConfirmCode(string userId);
    public Task ConfirmUser(ConfirmCodeDto confirmCodeDto);
    public Task RecoverPassword(string userEmail);
    public Task RecoveryUpdatePassword(RecoveryUpdatePasswordDto recoveryUpdatePasswordDto);
}   