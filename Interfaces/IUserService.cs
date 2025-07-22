using GameLogBack.Dtos;

namespace GameLogBack.Interfaces;

public interface IUserService
{
    public string RegisterUser(RegisterNewUserDto registerNewUser);
    public void UpdateUser(UpdateUserDto updateUserDto, string userId);
    public GetUserDto GetUser(string userId);
    public void ResendNewConfirmCode(string userId);
    public void ConfirmUser(ConfirmCodeDto confirmCodeDto);
}