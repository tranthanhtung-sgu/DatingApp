using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] public string KnownAs { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }

        [Required] public string City { get; set; }

        [Required] public string Country { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}