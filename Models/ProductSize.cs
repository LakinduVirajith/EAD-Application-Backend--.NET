﻿using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductSize
    {
        [Key]
        public int SizeId { get; set; }

        [Required]
        [StringLength(20)]
        public string Size { get; set; } = string.Empty;

        // FOREIGN KEY TO PRODUCT
        public string ProductId { get; set; } = string.Empty;
        public virtual ProductModel Product { get; set; }

        public ProductSize()
        {
            
        }
    }
}