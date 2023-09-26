using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Movies_website_project.Entities
{
    public class Actor
    {
        public int id { get; set; }
        public string name { get; set; }
        [NotMapped]
        public bool editable { get; set; }
        [NotMapped]
        public bool visible { get; set; }
        [JsonIgnore]
        public ICollection<Movie>? Movies { get; set; }
    }
}
