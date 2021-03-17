using Microsoft.AspNetCore.Http;
using System.IO;

namespace CrilieTechReviewsBlog.DataManagement.FileManager
{
    public interface IFileManager
    {
        FileStream ImageStream(string image);
        string SaveImage(IFormFile image);
        bool RemoveImage(string image);
    }
}
