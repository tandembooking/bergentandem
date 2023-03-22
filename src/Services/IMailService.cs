using System.Threading.Tasks;

namespace TandemBooking.Services
{
    public interface IMailService
    {
        Task Send(string recipient, string subject, string body);
    }
}