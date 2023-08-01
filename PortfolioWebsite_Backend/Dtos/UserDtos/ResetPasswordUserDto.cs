﻿using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class ResetPasswordUserDto
    {
        public required string Password { get; set; }
        [Required, Compare("Password")]
        public required string PasswordConfirmation { get; set; }
    }
}
