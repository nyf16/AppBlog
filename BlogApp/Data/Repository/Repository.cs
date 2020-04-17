using BlogApp.Helpers;
using BlogApp.Models;
using BlogApp.Models.Comments;
using BlogApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IndexViewModel GetAllPosts(int pageNumber, string category, string search)
        {
            Expression<Func<Post, bool>> InCategory = (post) => post.Category.ToLower().Equals(category.ToLower());

            int pageSize = 2;
            int skipAmount = pageSize * (pageNumber - 1);

            //IQueryable<Post> 
            var query = _ctx.Posts.AsNoTracking().AsQueryable();
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.ToLower() == category.ToLower());
            //query = query.Where(x => InCategory(x));

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{ search}%")
                                    || EF.Functions.Like(x.Body, $"%{ search}%")
                                    || EF.Functions.Like(x.Description, $"%{ search}%"));

            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
                Category = category,
                Search = search,
                Posts = query
                     .Skip(skipAmount)
                     .Take(pageSize)
                     .ToList()

            };

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

