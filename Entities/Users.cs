using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Entities;

public class Users
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserEmail { get; set; }
    public UserLogins UserLogins { get; set; }
}