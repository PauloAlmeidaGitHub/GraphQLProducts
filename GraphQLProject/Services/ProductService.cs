using GraphQLProject.Data;
using GraphQLProject.Interfaces;
using GraphQLProject.Models;
using System.Collections.Generic;
using System.Linq;

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


        private GraphQLDbContext _dbContext;
        public ProductService(GraphQLDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public List<Product> GetAllProducts()
        {
            //return products;

            return _dbContext.Products.ToList();    //using System.Linq;
        }

        public Product AddProduct(Product product)
        {
            //products.Add(product); 
            //return product;

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
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