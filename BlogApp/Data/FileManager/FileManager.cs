using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> SavePicture(IFormFile picture)
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
                    await picture.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
        }
    }
}