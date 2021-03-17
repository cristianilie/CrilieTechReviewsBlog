using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoSauce.MagicScaler;
using System;
using System.IO;

namespace CrilieTechReviewsBlog.DataManagement.FileManager
{
    public class FileManager : IFileManager
    {
        private string _imagePath;
        private readonly ILogger<FileManager> _logger;


        public FileManager(IConfiguration configuration, ILogger<FileManager> logger)
        {
            _imagePath = configuration["Path:Images"];
            _logger = logger;
        }

        public FileStream ImageStream(string image) => new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);

                if (File.Exists(file))
                    File.Delete(file);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return false;
            }
        }

        public string SaveImage(IFormFile image)
        {
            try
            {
                var saveImgPath = Path.Combine(_imagePath);

                if (!Directory.Exists(saveImgPath))
                    Directory.CreateDirectory(saveImgPath);

                var imgType = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                var fileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{imgType}";

                using (var fileStream = new FileStream(Path.Combine(saveImgPath, fileName), FileMode.Create))
                {
                    MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, ImageSettings());
                };

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return $"Error => {ex.Message}";
            }
        }

        private ProcessImageSettings ImageSettings()
            => new ProcessImageSettings
            {
                Width = 800,
                Height = 400,
                SaveFormat = FileFormat.Jpeg,
                ResizeMode = CropScaleMode.Crop,
                JpegQuality = 100,
                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
    }
}
