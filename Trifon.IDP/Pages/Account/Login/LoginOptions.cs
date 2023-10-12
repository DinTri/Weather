namespace Trifon.IDP.Pages.Account.Login;

public static class LoginOptions
{
    public static bool AllowLocalLogin { get; } = true;

    public static bool AllowRememberLogin { get; } = true;

    public static TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

    public static string InvalidCredentialsErrorMessage { get; } = "Invalid username or password";
}
