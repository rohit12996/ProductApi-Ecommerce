using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interface;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //check if the product already exist
                var getProduct = await GetByAsync( _ => _.Name!.Equals(entity.Name) );
                if (getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already added");

                var currencyEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currencyEntity != null && currencyEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added to database successfully ");
                else
                    return new Response(false, $" erroe while adding {entity.Name}");
            }
            catch (Exception ex)
            {
                //Log Exception 
                LogException.LogExceptions(ex);
                return new Response(false, "Error Occoured adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await GetByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not found");
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                 return new Response(true, $"{entity.Name}  is deleted successfully");

            }
            catch (Exception ex)
            {
                //Log Exception
                LogException.LogExceptions(ex);
                return new Response(false, "Error Occoured while deleting product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;

            }
            catch (Exception ex)
            {

                //Log Exception
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occoured retriving product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log Exception
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occoured retriving product");
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;


            }
            catch (Exception ex)
            {

                //Log Exception
                LogException.LogExceptions(ex);
                throw new Exception("Error occoured retriving product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await GetByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not found");
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} updated successfully");

            }
            catch (Exception ex)
            {


                //Log Exception
                LogException.LogExceptions(ex);
                return new Response(false , "Error occoured updating existing  product");
            }
        }
    }
}
