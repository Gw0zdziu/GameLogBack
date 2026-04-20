using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos.User;

public class RegisterNewUserDto
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserEmail { get; set; }

    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public string InvitationCode { get; set; }
}
