using System.ComponentModel.DataAnnotations;

namespace AzureServiceBus.Models
{
    public class Product
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Price { get; set; }
    }
}