using System.Threading.Tasks;

namespace SlackNotifierWS.Service
{
    public interface ISlackNotifierService
    {
        public Task<bool> NotifyAsync(string url, string message);
    }
}