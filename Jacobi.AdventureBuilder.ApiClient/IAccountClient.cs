namespace Jacobi.AdventureBuilder.ApiClient;

public interface IAccountClient
{
    void Signup(SignupRequest request);
    LoginResponse Login(LoginRequest request);
}

public record LoginRequest(string Email, string Password);
public record LoginResponse(string UserToken);

public record SignupRequest(string Name, string Email, string? Nickname);