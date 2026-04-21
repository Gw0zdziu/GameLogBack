using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos.User;

public class ConfirmCodeDto
{
    public string UserId { get; set; }

    public string ConfirmCode { get; set; }
}
