using CrilieTechReviewsBlog.DataManagement.FileManager;
using CrilieTechReviewsBlog.DataManagement.Repositories;
using CrilieTechReviewsBlog.Models;
using CrilieTechReviewsBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.Controllers
{
    public class HomeController : Controller
    {
        IPostRepository _postRepository;
        IFileManager _fileManager;

        private readonly ILogger<HomeController> _logger;

        public HomeController(IPostRepository postRepository, IFileManager fileManager, ILogger<HomeController> logger)
        {
            _postRepository = postRepository;
            _fileManager = fileManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult About() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int pageNumber, string search)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, search });

            try
            {
                IndexViewModel indexVM = _postRepository.GetAllPosts(pageNumber, search) ??   new IndexViewModel
                {
                    PageNumber = pageNumber,
                    PageCount = 0,
                    Pages = new List<int>(),
                    Search = search,
                    Posts = new List<Post>()
                }; 

                return View(indexVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("Index", new { pageNumber = 1, search });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Post(int Id)
        {
            var post = _postRepository.GetPost(Id);

            if (post == null)
                return RedirectToAction("ResourceNotFound", "Error", new { errorName = "Post" });

            return View(post);
        }

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        {
            try
            {
                string imgType = image.Substring(image.LastIndexOf('.') + 1);
                return new FileStreamResult(_fileManager.ImageStream(image), $"image/{imgType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return null;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(CommentViewModel commentVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("Post", new { Id = commentVM.PostId });

                Post post = _postRepository.GetPost(commentVM.PostId);
                string userName = User.FindFirstValue(ClaimTypes.Name);

                if (commentVM.MainCommentId == 0)
                {
                    post.MainComments = post.MainComments ?? new List<MainComment>();
                    post.MainComments.Add(
                        new MainComment
                        {
                            Message = commentVM.Message,
                            CreationDate = DateTime.Now,
                            UserName = userName
                        });
                    await _postRepository.UpdatePost(post);
                }
                else
                {
                    var subComment = new SubComment
                    {
                        MainCommentId = commentVM.MainCommentId,
                        Message = commentVM.Message,
                        CreationDate = DateTime.Now,
                        UserName = userName
                    };
                    await _postRepository.AddSubComment(subComment);
                }
                return RedirectToAction("Post", new { Id = commentVM.PostId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditComment(int commentId, int postId)
        {
            try
            {
                var post = _postRepository.GetPost(postId);
                Comment comment = null;

                if (post != null)
                    comment = post.MainComments.Where(c => c.Id == commentId).FirstOrDefault(); ;

                if (comment == null && post != null)
                {
                    foreach (var comm in post.MainComments)
                    {
                        if (comm.SubComments.Where(s => s.Id == commentId).Count() > 0)
                        {
                            comment = comm.SubComments.FirstOrDefault(s => s.Id == commentId);
                            break;
                        }
                    }

                    CommentViewModel commentViewModel = new CommentViewModel
                    {
                        Id = comment.Id,
                        PostId = postId,
                        UserId = comment.UserId,
                        Message = comment.Message
                    };
                    return View(commentViewModel);
                }

                if (comment != null && post != null)
                {
                    CommentViewModel commentViewModel = new CommentViewModel
                    {
                        Id = comment.Id,
                        PostId = postId,
                        UserId = comment.UserId,
                        Message = comment.Message
                    };
                    return View(commentViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditComment(CommentViewModel commentVM)
        {
            if (commentVM != null)
            {
                try
                {
                    await _postRepository.UpdateComment(commentVM.Id, commentVM.PostId, commentVM.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.StackTrace);
                    return RedirectToAction("SomethingWentWrong", "Error");
                }

                return RedirectToAction("Post", new { Id = commentVM.PostId });
            }
            else
            {
                return RedirectToAction("ResourceNotFound", "Error", new { errorName = "Post" });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {
            if (commentId != 0)
            {
                try
                {
                    await _postRepository.DeleteComment(commentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.StackTrace);
                    return RedirectToAction("SomethingWentWrong", "Error");
                }
                return RedirectToAction("Post", new { Id = postId });
            }
            else
            {
                return RedirectToAction("ResourceNotFound", "Error", new { errorName = "Post" });
            }
        }
    }
}
