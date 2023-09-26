using System.ComponentModel.DataAnnotations.Schema;

namespace Movies_website_project.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        [ForeignKey("MovieId")]
        public int MovieId { get; set; }
        public string ReviewDetails { get; set;}
    }
}
