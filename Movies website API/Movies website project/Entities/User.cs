namespace Movies_website_project.Entities
{
    public class User
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
