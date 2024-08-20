﻿using OrderService.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderService.API.Models.DTO
{
    public class OrderDetailsDto
    {
      
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }      

        public int ProductId { get; set; }
       
        public ProductDto? Product { get; set; }

        public int ProductCount { get; set; }
        public string productName { get; set; }
        public double Price { get; set; }
    }
}
