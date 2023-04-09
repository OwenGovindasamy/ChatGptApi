using ChatGptApi.Models;

namespace ChatGptApi.Interfaces
{
    public interface IImageLogic
    {
        Task<string> CreateImage(ImageProperties image);
    }
}
