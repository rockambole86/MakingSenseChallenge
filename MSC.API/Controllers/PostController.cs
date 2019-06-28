using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSC.API.Dto;
using MSC.API.Models;
using MSC.API.Services;

namespace MSC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAny")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger      _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger      = logger;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Post>> Get()
        {
            try
            {
                var list = _postService.GetPosts();

                return Ok(list);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "PostController - Get()");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Post> Get(int id)
        {
            try
            {
                var post = _postService.GetPost(id);

                if (post == null)
                    return NotFound();

                return Ok(post);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "PostController - Get(int id)");

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePostRequest rq)
        {
            try
            {
                var postId = _postService.CreatePost(rq);

                //return Ok(postId);

                var post = _postService.GetPost(postId);

                return Ok(post);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "PostController - Create(CreatePostRequest rq)");

                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _postService.DeletePost(id);

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "PostController - Delete(int id)");

                return BadRequest();
            }
        }
    }
}
