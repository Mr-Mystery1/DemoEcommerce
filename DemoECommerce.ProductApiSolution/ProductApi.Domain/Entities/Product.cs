﻿namespace ProductApi.Domain.Entities
{
    public class Product
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
    }
}
