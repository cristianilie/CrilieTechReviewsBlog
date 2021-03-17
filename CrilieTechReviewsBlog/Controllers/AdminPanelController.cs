using CrilieTechReviewsBlog.DataManagement.FileManager;
using CrilieTechReviewsBlog.DataManagement.Repositories;
using CrilieTechReviewsBlog.Models;
using CrilieTechReviewsBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminPanelController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileManager _fileManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlackListedUserRepository _blackListedUser;
        private readonly ILogger<AdminPanelController> _logger;
        public AdminPanelController(IPostRepository postRepository,
                                    IFileManager fileManager,
                                    ICategoryRepository categoryRepository,
                                    UserManager<IdentityUser> userManager,
                                    IBlackListedUserRepository blackListedUser,
                                    ILogger<AdminPanelController> logger)
        {
            _postRepository = postRepository;
            _fileManager = fileManager;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _blackListedUser = blackListedUser;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Post() => View("Edit", new PostViewModel());

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Post(PostViewModel postVM)
        {
            try
            {
                if (postVM != null)
                {
                    Post post = new Post
                    {
                        Id = postVM.Id,
                        Title = postVM.Title,
                        Summary = postVM.Summary,
                        Body = postVM.Body,
                        Category = postVM.Category
                    };

                    if (postVM.Image == null)
                    {
                        post.Image = postVM.CurrentImage;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(postVM.CurrentImage))
                            _fileManager.RemoveImage(postVM.CurrentImage);

                        post.Image = _fileManager.SaveImage(postVM.Image);
                    }

                    int newPostId = await _postRepository.AddPost(post);

                    if (newPostId > 0)
                        return RedirectToAction("Post", "Home", new { Id = newPostId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }

            return View("Edit", new PostViewModel());
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return View(new PostViewModel() { Categories = _categoryRepository.GetAllCategories() });
                }
                else
                {
                    var post = _postRepository.GetPost((int)id);
                    return View(new PostViewModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Summary = post.Summary,
                        Body = post.Body,
                        Category = post.Category,
                        Categories = _categoryRepository.GetAllCategories(),
                        CurrentImage = post.Image
                    });
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
        public async Task<IActionResult> Edit(PostViewModel postVM)
        {
            try
            {
                if (postVM != null)
                {
                    Post post = new Post
                    {
                        Id = postVM.Id,
                        Title = postVM.Title,
                        Summary = postVM.Summary,
                        Body = postVM.Body,
                        Category = postVM.Category
                    };

                    if (postVM.Image == null)
                    {
                        post.Image = postVM.CurrentImage;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(postVM.CurrentImage))
                            _fileManager.RemoveImage(postVM.CurrentImage);

                        post.Image = _fileManager.SaveImage(postVM.Image);
                    }

                    await _postRepository.UpdatePost(post);

                    if (post.Id > 0)
                        return RedirectToAction("Post", "Home", new { Id = post.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                if (Id > 0)
                    await _postRepository.RemovePost(Id);
                else
                    return RedirectToAction("SomethingWentWrong", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult UserList()
        {
            try
            {
                List<BlacklistedUser> blacklistedUsers = _blackListedUser.GetBlacklistedUsers().ToList();
                List<UserListViewModel> userList = _userManager.Users.Select(u => new UserListViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                }).ToList();

                foreach (var blUser in blacklistedUsers)
                {
                    userList.Where(u => u.UserId == blUser.UserId).FirstOrDefault().IsBlackListed = true;
                }

                return View(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("SomethingWentWrong", "Error");

        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var blackListed = _blackListedUser.GetBlacklistedUser(userId);
                var signedInUser = await _userManager.GetUserAsync(HttpContext.User);

                if (user != null && user.Email != signedInUser.Email)
                {
                    if (blackListed != null)
                        await _blackListedUser.RemoveUserFromBlackList(blackListed);

                    await _userManager.DeleteAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("UserList");

        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> AddToBlackList(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null && await _userManager.IsInRoleAsync(user, "Administrator") == false)
                    await _blackListedUser.AddUserToBlackList(new BlacklistedUser { UserId = userId });
                else
                    return RedirectToAction("SomethingWentWrong", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("UserList");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> RemoveFromBlackList(string userId)
        {
            try
            {
                var user = _blackListedUser.GetBlacklistedUser(userId);

                if (user != null)
                    await _blackListedUser.RemoveUserFromBlackList(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("UserList");
        }
    }
}
