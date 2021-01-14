using EcommerceHomework.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceHomework.Models
{
    public class User
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public List<CardItem> Basket { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        ADMIN = 1,
        USER = 0
    }
}
