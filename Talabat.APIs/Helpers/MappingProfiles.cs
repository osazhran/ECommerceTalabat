using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using UserAdder = Talabat.Core.Entities.Identity.Address;
using OrderAddres = Talabat.Core.Entities.Order_Aggregate.Address;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, O => O.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.Category, O => O.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<CustomerBasketDto, CustomerBasket>();

            CreateMap<AddressDto, OrderAddres>().ReverseMap();

            CreateMap<UserAdder, AddressDto>()
                .ForMember(d => d.FirstName, O => O.MapFrom(S => S.FName))
                .ForMember(d => d.LastName, O => O.MapFrom(S => S.LName)).ReverseMap();


            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DelivaryMethod, O => O.MapFrom(s => s.DelivaryMethod.ShortName))
                .ForMember(d => d.DelivaryMethodCost, O => O.MapFrom(s => s.DelivaryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());



        }
    }
}
