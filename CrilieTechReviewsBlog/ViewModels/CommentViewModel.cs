namespace CrilieTechReviewsBlog.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int MainCommentId { get; set; }
        public string Message { get; set; }

    }
}
