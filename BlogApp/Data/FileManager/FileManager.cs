using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSauce.MagicScaler;

namespace BlogApp.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private string _picturePath;

        public FileManager(IConfiguration config)
        {
            _picturePath = config["Path:Pictures"];
        }

        public FileStream PictureStream(string picture)
        {
            return new FileStream(Path.Combine(_picturePath, picture), FileMode.Open, FileAccess.Read);
        }

        public bool RemovePicture(string picture)
        {
            try
            {
                var file = Path.Combine(_picturePath, picture);
                if (File.Exists(file))
                    File.Delete(file);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        public string SavePicture(IFormFile picture)
        {
            try
            {
                var save_path = Path.Combine(_picturePath);
                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }

                //Internet Explorer Error C:/User/Foo/image.jpg
                //var fileName = picture.FileName;
                var mime = picture.FileName.Substring(picture.FileName.LastIndexOf('.'));
                var fileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";

                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    //await picture.CopyToAsync(fileStream);
                    MagicImageProcessor.ProcessImage(picture.OpenReadStream(), fileStream, PictureOptions());
                }

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
        }
        private ProcessImageSettings PictureOptions() => new ProcessImageSettings
        {
            Width = 800,
            Height = 500,
            ResizeMode = CropScaleMode.Crop,
            SaveFormat = FileFormat.Jpeg,
            JpegQuality = 100,
            JpegSubsampleMode = ChromaSubsampleMode.Subsample420
        };

        Task<string> IFileManager.SavePicture(IFormFile picture)
        {
            throw new NotImplementedException();
        }
    }
}