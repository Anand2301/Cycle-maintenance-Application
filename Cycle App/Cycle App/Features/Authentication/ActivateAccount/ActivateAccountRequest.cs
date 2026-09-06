namespace CycleMaintenance.Features.Authentication.ActivateAccount;

public class ActivateAccountRequest
{
    public string Token { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}