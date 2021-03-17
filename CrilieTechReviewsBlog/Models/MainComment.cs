using System.Collections.Generic;

namespace CrilieTechReviewsBlog.Models
{
    public class MainComment : Comment
    {
        public List<SubComment> SubComments { get; set; }
    }
}
