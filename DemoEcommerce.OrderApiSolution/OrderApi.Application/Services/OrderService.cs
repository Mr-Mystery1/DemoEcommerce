﻿using System.Net.Http.Json;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversion;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface,HttpClient httpClient,ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        
        // GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call Product API using HttpClient
            // Redirect this call to the API Gateway since product Api does not response to outsiders..
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if(!getProduct.IsSuccessStatusCode)
                return null!;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }
        
        // GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call Product API using HttpClient
            // Redirect this call to the API Gateway since Product Api does not response to outsiders..
            var getUser = await httpClient.GetAsync($"/api/authentication/{userId}");

            //var getUser = await httpClient.GetAsync($"http://localhost:5000/api/Authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return product!;

        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
                return null!;

            // Get Retry Pipeline  
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare Client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate Order Details

            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelePhoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );

        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all Client's orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);

            if (!orders.Any()) 
                return null!;

            // Convert from Entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
