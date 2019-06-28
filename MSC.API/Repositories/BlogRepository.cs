using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.Logging;
using MSC.API.Context;
using MSC.API.Dto;
using MSC.API.Helpers;
using MSC.API.Models;

namespace MSC.API.Repositories
{
    public interface IBlogRepository
    {
        Post GetPost(int postId);
        IEnumerable<Post> GetPosts();
        int CreatePost(CreatePostRequest rq);
        void DeletePost(int postId);
    }

    public class BlogRepository : IBlogRepository
    {
        private readonly AppContext DB;
        private readonly ILogger    _logger;

        public BlogRepository(AppContext context, ILogger<BlogRepository> logger)
        {
            DB      = context;
            _logger = logger;
        }


        public Post GetPost(int postId)
        {
            var post = DB.Post.Find(postId);

            if (post != null)
            {
                var pt = DB.PostTag.Where(x => x.PostId == post.PostId);

                if (pt.Any())
                {
                    post.Tags = new List<Tag>();

                    foreach (var item in pt)
                    {
                        post.Tags.Add(DB.Tag.FirstOrDefault(x => x.TagId == item.TagId));
                    }
                }
            }

            return post;
        }

        public IEnumerable<Post> GetPosts()
        {
            var posts = DB.Post
                .Where(x => x.Status == (int) PostStatus.Public);

            foreach (var post in posts)
            {
                var pt = DB.PostTag.Where(x => x.PostId == post.PostId);

                if (pt.Any())
                {
                    post.Tags = new List<Tag>();

                    foreach (var item in pt)
                    {
                        post.Tags.Add(DB.Tag.FirstOrDefault(x => x.TagId == item.TagId));
                    }
                }
            }

            return posts;
        }

        public int CreatePost(CreatePostRequest rq)
        {
            var post = new Post{
                Html = rq.Html,
                Status = (int) PostStatus.Public,
                CreateBy = 0
            };

            DB.Post.Add(post);

            DB.SaveChanges();

            if (rq.Tags != null && rq.Tags.Count() > 0)
            {
                foreach (var t in rq.Tags)
                {
                    var dbTag = DB.Tag.FirstOrDefault(x => x.Name.Equals(t, System.StringComparison.OrdinalIgnoreCase));

                    if (dbTag != null)
                    {
                        DB.PostTag.Add(new PostTag
                        {
                            PostId = post.PostId,
                            TagId = dbTag.TagId
                        });
                    }
                    else
                    {
                        var tag = new Tag{
                            Name = t
                        };

                        DB.Tag.Add(tag);

                        DB.SaveChanges();

                        DB.PostTag.Add(new PostTag
                        {
                            PostId = post.PostId,
                            TagId = tag.TagId
                        });
                    }

                    DB.SaveChanges();
                }
            }

            return post.PostId;
        }

        public void DeletePost(int postId)
        {
            var post = DB.Post.Find(postId);

            DB.Post.Remove(post);

            var relations = DB.PostTag.Where(x => x.PostId == postId);

            DB.PostTag.RemoveRange(relations);

            DB.SaveChanges();
        }
    }
}
