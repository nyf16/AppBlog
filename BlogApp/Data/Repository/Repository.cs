using BlogApp.Models;
using BlogApp.Models.Comments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);

        }

        public List<Post> GetAllPosts()
        {

            return _ctx.Posts.ToList();
        }

        public List<Post> GetAllPosts(int pageNumber)
        {
            int pageSize = 5;
            int pageCount = _ctx.Posts.Count() / pageSize;
            return _ctx.Posts
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();
        }

        public List<Post> GetAllPosts(string category)
        {

            return _ctx.Posts.ToList();
            //Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };
            //.Where(post => post.Category.ToLower().Equals(category.ToLower()))
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts
                .Include(p => p.MainComments)
                .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }
    }
}
