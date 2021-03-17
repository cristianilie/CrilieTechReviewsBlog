using CrilieTechReviewsBlog.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CrilieTechReviewsBlog.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Summary { get; set; } = "";
        public string CurrentImage { get; set; } = "";
        public IFormFile Image { get; set; } = null;
        public int Category { get; set; }
        public List<Category> Categories { get; set; }
    }
}
