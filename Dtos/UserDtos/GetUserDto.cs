﻿using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class GetUserDto
    {
        public int Id { get; set; }
        [Required]
        public required string UserName { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Role { get; set; }
        [Timestamp]
        public DateTime CreatedAt { get; set; }
        [Timestamp]
        public DateTime UpdatedAt { get; set; }
    }
}
