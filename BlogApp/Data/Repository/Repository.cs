using BlogApp.Models;
using BlogApp.Models.Comments;
using BlogApp.ViewModels;
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

        public IndexViewModel GetAllPosts(int pageNumber, string category)
        {
            Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            int pageSize = 1;
            int skipAmount = pageSize * (pageNumber - 1);

            var query = _ctx.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => InCategory(x));

            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageNumbers(pageNumber, pageCount),
                Category = category,
                Posts = query
                     .Skip(skipAmount)
                     .Take(pageSize)
                     .ToList()

            };

        }

        private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {            
            int midPoint = pageNumber < 3 ? 3
                : pageNumber > pageCount - 2 ? pageCount - 2
                : pageNumber;

            int lowerBound = midPoint - 2;
            int upperBound = midPoint + 2;

            if (lowerBound != 1)
            {
                yield return 1;
                if (lowerBound - 1 > 1)
                {
                    yield return -1;
                }

            }

            for (int i = midPoint - 2; i <= midPoint + 2; i++)
            {
                yield return i;
            }

            if (upperBound != pageCount)
            {
                if (pageCount - upperBound > 1)
                {
                    yield return -1;
                }
                yield return pageCount;
            }
            
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

