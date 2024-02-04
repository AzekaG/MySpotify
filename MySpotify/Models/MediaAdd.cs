using MySpotify.Repository;
using System.ComponentModel.DataAnnotations;

namespace MySpotify.Models
{
   
    
    public class MediaAdd
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Artist { get; set; }
        
        public string? FileAdress { get; set; }
        
        public string? Poster { get; set; }

        public Genre? genre { get; set; }
        
        [Required]
        public TypeMedia typeMedia { get; set; }

          }
        }






