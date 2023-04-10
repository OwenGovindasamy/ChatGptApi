using ChatGptApi.Models;

namespace ChatGptApi.Interfaces
{
    public interface IChatLogic
    {
        Task<string> CreateChat(string chat);
    }
}
