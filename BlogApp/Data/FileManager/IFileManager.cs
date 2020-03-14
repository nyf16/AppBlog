using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.FileManager
{
    public interface IFileManager
    {
        FileStream PictureStream(string picture);
        Task<string> SavePicture(IFormFile picture);
        bool RemovePicture(string picture);
    }
}
