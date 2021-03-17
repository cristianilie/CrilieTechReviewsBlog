using System;

namespace CrilieTechReviewsBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
