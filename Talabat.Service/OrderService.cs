﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {     
        private   readonly IBasketRepository _basketRepo;
        private   readonly IUnitOfWork _unitOfWork;
        private   readonly IPaymentService _paymentService;

        ///private readonly IGenericRepository<Product> _productRepo;
        ///private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        ///private readonly IGenericRepository<Order> _ordersRepo;

        public OrderService(
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService
            ///IGenericRepository<Product> productRepo,
            ///IGenericRepository<DeliveryMethod> deliveryMethodRepo,
            ///IGenericRepository<Order> ordersRepo
            )
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            ///_productRepo = productRepo;
            ///_deliveryMethodRepo = deliveryMethodRepo;
            ///_ordersRepo = ordersRepo;
        }

        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address ShippingAddress)
        {
            // 1. Get Basket From Baskets Repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();

            if(basket?.Items?.Count > 0)
            {
                var productRepo = _unitOfWork.Repository<Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetByIdAsync(item.Id);

                    var productItemOrder = new ProductItemOrdered(item.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrder, product.Price, item.Quantity);

                    orderItems.Add(orderItem);

                }
            }
            // 3. Calculate SubTotal
            var subTotal = orderItems.Sum(orderItem => orderItem.Price *  orderItem.Quantity);

            // 4. Get Delivery Method From DeliveryMethods Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var OrderRepo = _unitOfWork.Repository<Order>();

            var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);

            var existingOrder = await OrderRepo.GetEntityWithSpecAsync(orderSpec);

            if (existingOrder != null)
            {
                OrderRepo.Delete(existingOrder);

                await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            }


            // 5. Create Order
            var order = new Order(buyerEmail, ShippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

            await OrderRepo.AddAsync(order);

            // 6. Save To Database [TODO]
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return null;
            
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUsersAsync(string buyerEmail)
        {
            var ordesRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecification(buyerEmail);

            var orders = await ordesRepo.GetAllWithSpecAsync(spec);

            return orders;
        }
        public async Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecification(orderId, buyerEmail);

            var order = await orderRepo.GetEntityWithSpecAsync(spec);  

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var DeliveryMerhodRepo = _unitOfWork.Repository<DeliveryMethod>();

            var deliveryMethods = await DeliveryMerhodRepo.GetAllAsync();

            return deliveryMethods;
        }
    }
}
