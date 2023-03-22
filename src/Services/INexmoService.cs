using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TandemBooking.Services
{
    public interface INexmoService
    {
        Task<string> FormatPhoneNumber(string phoneNumber, string countryCode = "NO");
        Task<JObject> LookupPhoneNumber(string phoneNumber, string countryCode = "NO");
        Task<NexmoSmsResult> SendSms(string from, string to, string text);
    }
}