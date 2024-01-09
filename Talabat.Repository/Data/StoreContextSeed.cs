using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext _DbContect)
        {
            if (_DbContect.ProductBrands.Count() == 0)
            {
                var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands?.Count() > 0)
                {
                    foreach (var brand in brands)
                    {
                        _DbContect.Set<ProductBrand>().Add(brand);
                    }
                    await _DbContect.SaveChangesAsync();
                } 
            }

            if (_DbContect.ProductCategories.Count() == 0)
            {
                var CategoriesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
                var Categories = JsonSerializer.Deserialize<List<ProductCategory>>(CategoriesData);

                if (Categories?.Count() > 0)
                {
                    foreach (var Category in Categories)
                    {
                        _DbContect.Set<ProductCategory>().Add(Category);
                    }
                    await _DbContect.SaveChangesAsync();
                } 
            }

            if (_DbContect.Products.Count() == 0)
            {
                var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(ProductsData);

                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        _DbContect.Set<Product>().Add(product);
                    }
                    await _DbContect.SaveChangesAsync();
                } 
            }

            if (_DbContect.DelivaryMethods.Count() == 0)
            {
                var DelivaryMethodsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                var DelivaryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DelivaryMethodsData);

                if (DelivaryMethods?.Count() > 0)
                {
                    foreach (var delivaryMethod in DelivaryMethods)
                    {
                        _DbContect.Set<DeliveryMethod>().Add(delivaryMethod);
                    }
                    await _DbContect.SaveChangesAsync();
                }
            }
        }
    }
}
