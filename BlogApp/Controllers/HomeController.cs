using BlogApp.Data;
using BlogApp.Data.FileManager;
using BlogApp.Data.Repository;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repo;
        private IFileManager _fileManager;

        public HomeController(
            IRepository repo,
            IFileManager fileManager
            )
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        public IActionResult Index(string category)
        {
            var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);
            // boolean ? true : false ;     1=1? run : ignore
            return View(posts);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);
            return View(post);
        }

        [HttpGet("/Picture/{picture}")]
        public IActionResult Picture(string picture)
        {
            var mime = picture.Substring(picture.LastIndexOf('.') + 1);
            return new FileStreamResult(_fileManager.PictureStream(picture), $"picture/{mime}");
        }
    }
}
