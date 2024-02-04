using System.ComponentModel.DataAnnotations;

namespace MySpotify.Models
{


 public  enum TypeMedia 
    {
      Video , 
      Audio 
    }

public class Media
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Artist {  get; set; }
       
        public string? FileAdress { get; set; }
        
        public string? Poster { get; set; }

        
        public Genre? Genre { get; set; }
        [Required]
        public TypeMedia TypeMedia { get; set; }

        public User User { get; set; }
        public Media() { }
    }
}
