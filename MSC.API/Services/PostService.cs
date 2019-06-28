using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSC.API.Dto;
using MSC.API.Models;
using MSC.API.Repositories;

namespace MSC.API.Services
{
    public interface IPostService
    {
        Post GetPost(int postId);
        IEnumerable<Post> GetPosts();
        int CreatePost(CreatePostRequest rq);
        void DeletePost(int postId);
    }

    public class PostService : IPostService
    {
        private readonly ILogger<PostService> _logger;
        private readonly IBlogRepository      _repository;


        public PostService(IBlogRepository repository, ILogger<PostService> logger)
        {
            _logger     = logger;
            _repository = repository;
        }



        public int CreatePost(CreatePostRequest rq)
        {
            return _repository.CreatePost(rq);
        }

        public void DeletePost(int postId)
        {
            _repository.DeletePost(postId);
        }

        public Post GetPost(int postId)
        {
            return _repository.GetPost(postId);
        }

        public IEnumerable<Post> GetPosts()
        {
            return _repository.GetPosts();
        }
    }
}
