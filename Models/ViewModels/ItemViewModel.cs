using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EcommerceHomework.Models.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }

        public IFormFile Photo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public string Category { get; set; }

        public string Description { get; set; }

        public byte[] PhotoBytes { get; set; }
    }
}
