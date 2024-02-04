namespace MySpotify.Models
{
    public class UsersForMapping
    {

        public int id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public bool Active { get; set; }
        public Status Status { get; set; }

    }
}
