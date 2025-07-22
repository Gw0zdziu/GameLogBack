namespace GameLogBack.Dtos;

public class GetUserDto
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserEmail { get; set; }
    public bool IsActive { get; set; }
}