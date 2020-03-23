using BlogApp.Data;
using BlogApp.Data.FileManager;
using BlogApp.Data.Repository;
using BlogApp.Models;
using BlogApp.Models.Comments;
using BlogApp.ViewModels;
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

        public IActionResult Index(string category) =>
            View(string.IsNullOrEmpty(category) ?
            _repo.GetAllPosts() :
            _repo.GetAllPosts(category));

        public IActionResult Post(int id) =>
            View(_repo.GetPost(id));

        [HttpGet("/Picture/{picture}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Picture(string picture) =>
            new FileStreamResult(
                _fileManager.PictureStream(picture), $"picture/{picture.Substring(picture.LastIndexOf('.') + 1)}");

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Post", new { id = vm.PostId });

            var post = _repo.GetPost(vm.PostId);
            if (vm.MainCommentId > 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,

                });

                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = vm.PostId });
        }

        //public IActionResult Index(string category)
        //{
        //    var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);
        //    // boolean ? true : false ;     1=1? run : ignore
        //    return View(posts);
        //}

        //public IActionResult Post(int id)
        //{
        //    var post = _repo.GetPost(id);
        //    return View(post);
        //}

        //[HttpGet("/Picture/{picture}")]
        //public IActionResult Picture(string picture)
        //{
        //    var mime = picture.Substring(picture.LastIndexOf('.') + 1);
        //    return new FileStreamResult(_fileManager.PictureStream(picture), $"picture/{mime}");
        //}
    }
}
