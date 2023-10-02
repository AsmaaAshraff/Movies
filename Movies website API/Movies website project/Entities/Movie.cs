using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace Movies_website_project.Entities
{
    public class Movie
    {
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }
        public string? Title { get; set; }
        [FromForm]
        [NotMapped]
        public IFormFile? PosterImage { get; set; }
        [NotMapped]
        public string? ActorsString {  get; set; }
        [NotMapped]
        public string? ReviewsString {  get; set; }
        public string? PosterPath { get; set; }
        public DateTime? ReleaseDate { get; set;}
        public ICollection<Actor>? Actors { get; set;}
        public ICollection<Review>? Reviews { get; set; }
        public byte[] ImageByteArray { get; set; }
        [NotMapped]
        public bool visible { get; set; }
    }
}
