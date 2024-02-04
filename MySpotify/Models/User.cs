namespace MySpotify.Models
{
   public enum Status
    {
        admin,
        user
    }
    public class User
    {
        public int id {  get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Salt { get; set; }

        public bool Active { get; set; }
        public Status Status { get; set; } = Status.user;

        public ICollection<Media>? MediaFiles { get; set; }

     

        public User() 
        {
            MediaFiles = new List<Media>();
        }

        public bool isActive()
        {
            return Active;
        }

    }
}
