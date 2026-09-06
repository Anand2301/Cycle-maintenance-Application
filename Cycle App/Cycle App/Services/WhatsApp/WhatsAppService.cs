using System.Net;

namespace Cycle_App.Services.WhatsApp;

public class WhatsAppService: IWhatsAppService
{
    public string GenerateChatUrl(string PhoneNumber,string Message)
    {
        var encodedmessage = WebUtility.UrlEncode(Message);
        return $"https://wa.me/{PhoneNumber}?text={encodedmessage}";
    }
}
