﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioWebsite_Backend.Models.ContactModel
{
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Message { get; set; }
        [Timestamp]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Timestamp]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Contact() { }

        public Contact(string? name, string email, string? phone, string? message)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Message = message;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Email)}: {Email}, {nameof(Phone)}: {Phone}, {nameof(Message)}: {Message}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
        }
    }
}
