namespace Cycle_App.Services.WhatsApp;

public interface IWhatsAppService
{
    string GenerateChatUrl(string PhoneNumber, string Message);
}
