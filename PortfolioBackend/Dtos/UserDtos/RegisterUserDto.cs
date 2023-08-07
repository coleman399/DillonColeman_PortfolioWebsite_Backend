﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PortfolioBackend.Dtos.UserDtos
{
    public class RegisterUserDto
    {
        [Required]
        public required string UserName { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Role { get; set; } = "User";
        [Required]
        public required string Password { get; set; }
        [Required, Compare("Password")]
        public required string PasswordConfirmation { get; set; }
        [JsonIgnore, Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
