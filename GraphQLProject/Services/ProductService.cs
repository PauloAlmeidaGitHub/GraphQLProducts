using GraphQLProject.Data;
using GraphQLProject.Interfaces;
using GraphQLProject.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQLProject.Services
{
    public class ProductService : IProduct
    {
        // OLD CODE
        //private static List<Product> products = new List<Product>
        //{
        //    new Product(){Id = 0, Name = "Pão", Price = 5},
        //    new Product(){Id = 1, Name = "Leite", Price = 4},
        //    new Product(){Id = 2, Name = "Café", Price = 10},
        //    new Product(){Id = 3, Name = "Manteiga", Price = 9},
        //    new Product(){Id = 4, Name = "Bolo", Price = 10},
        //};


        // SHARED VARIABLES
        //===================================================================================
        string key = "ProductKey";
        List<Product> objectList = new List<Product>();
        string serializedObjectList;


        //CONSTRUCTORS
        //===================================================================================
        private readonly GraphQLDbContext _dbContext;
        private readonly IDistributedCache _distributedCache;
        public ProductService(GraphQLDbContext dbContext, IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _dbContext = dbContext;
        }


        //===================================================================================
        public List<Product> GetAllProducts()
        {
            //return _dbContext.Products.ToList();    //using System.Linq;

            // GET CACHE
            var cache = _distributedCache.GetString(key);
            if (cache != null)
            {
                // OBTEM DADOS NO CACHE
                var redisObjectList = _distributedCache.Get(key);
                serializedObjectList = Encoding.UTF8.GetString(redisObjectList);
                objectList = JsonConvert.DeserializeObject<List<Product>>(serializedObjectList);
            }
            else
            {
                // ALIMENTA O CACHE
                objectList = _dbContext.Products.ToList();
                serializedObjectList = JsonConvert.SerializeObject(objectList);
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _distributedCache.SetString(key, serializedObjectList);
            }
            return objectList;
            /*
            {
                  products{
                    id, name, price
                  }
                } 
            */
        }


        //===================================================================================
        public Product AddProduct(Product product)
        {
            //products.Add(product); 
            //return product;
           

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            // ALIMENTA O CACHE
                objectList = _dbContext.Products.ToList();
                serializedObjectList = JsonConvert.SerializeObject(objectList);
                var redisProductList = Encoding.UTF8.GetBytes(serializedObjectList);

                // DEFINE PRAZO DE EXPIRACAO PARA O CACHE (10min Janela Expiração 2min)
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _distributedCache.SetString(key, serializedObjectList);
            
            return product;
        }
        /*
            mutation AddProduct($product:ProductInputType){
             createProduct(product:$product) {
                    id, 
                    name, 
                    price
                }
             }

            {
              "product": {
                "id": 5,
                "name": "Costela Minga",
                "price": 60.75
              }
            }
        */

        //===================================================================================
        public Product UpdateProduct(int id, Product product)
        {
            //products[id] = product;
            //return product;

            // var productObjAUX = _dbContext.Products.FirstOrDefault(p => p.Id == id);  //TESTAR
            var productObjAUX = _dbContext.Products.Find(id);

            productObjAUX.Name = product.Name;
            productObjAUX.Price = product.Price;

            _dbContext.SaveChanges();
            return product;
        }
        /*
            mutation UpdateProduct($id: Int, $product: ProductInputType) {
            updateProduct(id: $id, product: $product) {
              id
              name
              price
            }
          }

            
          {
            "id": 2,
            "product": {
              "id": 2,
              "name": "Mortadela",
              "price": 3.34
	          }
          } 
        */

        //===================================================================================
        public void DeleteProduct(int id)
        {
            //products.RemoveAt(id);


            var productObjAUX = _dbContext.Products.Find(id);
            _dbContext.Remove(productObjAUX);
            _dbContext.SaveChanges();
        }
        /*
            mutation DeleteProduct($id:Int){
            deleteProduct(id:$id)
            }

            {
            "id": 5
            }
        */

        public Product GetProductById(int id)
        {
            //return products.Find(p => p.Id == id);

            // return _dbContext.Products.FirstOrDefault(p => p.Id == id);    // Testar - Nesse caso vazio não vai gerar exception
            return _dbContext.Products.Find(id);
        }
    }
}