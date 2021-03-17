using CrilieTechReviewsBlog.Models;
using System.Collections.Generic;

namespace CrilieTechReviewsBlog.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public string Search { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<int> Pages { get; internal set; }
    }
}
