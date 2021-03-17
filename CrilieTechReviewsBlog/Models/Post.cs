using System;
using System.Collections.Generic;

namespace CrilieTechReviewsBlog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Image { get; set; } = "";
        public int Category { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public List<MainComment> MainComments { get; set; }
    }
}
