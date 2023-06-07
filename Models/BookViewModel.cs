using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackOffice.Models
{
    public class BookViewModel
    {

        public int BookID { get; set; }
        public string Titre { get; set; }
        public string Author { get; set; }
        public string ReleaseDate { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string FileName { get; set; }
        public IFormFile ProfileImage { get; set; }
        public IFormFile File { get; set; }
    }
}
