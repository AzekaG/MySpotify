using Microsoft.AspNetCore.Mvc.Rendering;

namespace MySpotify.Models
{
    public class MediaGenreUserModelView
    {
        public Media medias { get; set; }
        public SelectList Genres { get; set; } = new SelectList(new List<Genre>(), "Id", "Name");

        public User user { get; set; }


    }
}
