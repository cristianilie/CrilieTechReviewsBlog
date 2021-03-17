using CrilieTechReviewsBlog.DataManagement.Repositories;
using CrilieTechReviewsBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public IActionResult Index() => View(_categoryRepository.GetAllCategories());

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                await _categoryRepository.RemoveCategory(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            try
            {
                return View(_categoryRepository.GetCategory(Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {

            if (id == null)
                return View(new Category());
            else
                return View(_categoryRepository.GetCategory((int)id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                if (category.Id > 0)
                {
                    await _categoryRepository.UpdateCategory(category);
                    return View("Details", _categoryRepository.GetCategory(category.Id));
                }
                else
                {
                    int categId = await _categoryRepository.AddCategory(category);
                    return View("Details", _categoryRepository.GetCategory(categId));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }
    }
}
