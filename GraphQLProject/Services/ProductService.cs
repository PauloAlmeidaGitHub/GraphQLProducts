using GraphQLProject.Data;
using GraphQLProject.Interfaces;
using GraphQLProject.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQLProject.Services
{
    public class ProductService : IProduct
    {
        //private static List<Product> products = new List<Product>
        //{
        //    new Product(){Id = 0, Name = "Pão", Price = 5},
        //    new Product(){Id = 1, Name = "Leite", Price = 4},
        //    new Product(){Id = 2, Name = "Café", Price = 10},
        //    new Product(){Id = 3, Name = "Manteiga", Price = 9},
        //    new Product(){Id = 4, Name = "Bolo", Price = 10},
        //};

        // VARIABLES


        //CONSTRUCTORS
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
            //return products;

            //return _dbContext.Products.ToList();    //using System.Linq;

            var cacheKey = "ProductKey";
            var products = new List<Product>();

            var json = _distributedCache.GetString(cacheKey);
            if (json != null)
            {
                // OBTEM DADOS NO CACHE
                var redisProductList = _distributedCache.Get(cacheKey);
                string serializedProductList = Encoding.UTF8.GetString(redisProductList);
                products = JsonConvert.DeserializeObject<List<Product>>(serializedProductList);
            }

            return products;
        }

        public Product AddProduct(Product product)
        {
            //products.Add(product); 
            //return product;
            string key = "ProductKey";
            string serializedProductList;
            var productList = new List<Product>();

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            // OBTEM DADOS NO CACHE
            var redisProductList = _distributedCache.Get(key);


            if (redisProductList != null)
            {
                // SE O CACHE POSSUI CONTEUDO
                serializedProductList = Encoding.UTF8.GetString(redisProductList);
                productList = JsonConvert.DeserializeObject<List<Product>>(serializedProductList);
            }
            else
            {
                // SE O CACHE ESTA VAZIO ALIMENTA O CACHE
                productList = _dbContext.Products.ToList();
                serializedProductList = JsonConvert.SerializeObject(productList);
                redisProductList = Encoding.UTF8.GetBytes(serializedProductList);
                _distributedCache.SetString(key, serializedProductList);
            }
            return product;

            
        }

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

        public void DeleteProduct(int id)
        {
            //products.RemoveAt(id);


            var productObjAUX = _dbContext.Products.Find(id);
            _dbContext.Remove(productObjAUX);
            _dbContext.SaveChanges();
        }

        public Product GetProductById(int id)
        {
            //return products.Find(p => p.Id == id);

            // return _dbContext.Products.FirstOrDefault(p => p.Id == id);    // Testar - Nesse caso vazio não vai gerar exception
            return _dbContext.Products.Find(id);
        }
    }
}


/*          JSON PARA TESTE
            
            // TODOS 
                {
                  products{
                    id, name, price
                  }
                }
  
            

             // INSERIR
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
                "name": "Ancho",
                "price": 6
            }
            



            // ATUALIZAR
            mutation UpdateProduct($id:Int $product:ProductInputType){
            updateProduct(id:$id product:$product) {
                   id, 
                   name, 
                   price
               }
            }

            
            {
              "id": 2,
              "product": {
                "id": 2,
                "name": "Mortadela",
                "price": 3
            }



            // DELETAR
            mutation DeleteProduct($id:Int){
            deleteProduct(id:$id)
            }

            {
            "id": 3
            }
          */