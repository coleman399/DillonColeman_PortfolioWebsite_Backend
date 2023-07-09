﻿using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class LoginUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required, Compare("Password")]
        public required string PasswordConfirmation { get; set; }
    }
}
