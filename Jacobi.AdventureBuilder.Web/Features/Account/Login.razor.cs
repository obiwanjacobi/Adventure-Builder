using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features.Account;

public partial class Login : ComponentBase
{
    private LoginModel loginModel = new LoginModel();

    private Task OnUserLogin()
    {
        // Access the values from the loginModel
        var email = loginModel.Email;
        var password = loginModel.Password;

        // Perform login logic here
        Console.WriteLine($"Email: {email}, Password: {password}");

        return Task.CompletedTask;
    }

    public sealed class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
