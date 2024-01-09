using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    internal static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {

        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery; // _dbContext.Set<Order>()

            if(spec.Criteria is not null) 
                query = query.Where(spec.Criteria);

            //  _dbContext.Set<Order>().Where(O => O.buyerEmail == Abdelramanzaky@gmail.com)
            if(spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            //query = query.GroupBy(spec.OrderBy) ;

            //  _dbContext.Set<Product>().Where( P => P.BrandId == 2 && true).OrderBy(P => P.Name)

            else if(spec.OrderByDesc is not null)
                query = query.OrderByDescending(spec.OrderByDesc);

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.SKip).Take(spec.Take);



            //  _dbContext.Set<Product>().OrderByDescending(P => P.Price)

            // Includes
            // 1. P => P.Brand
            // 2. P => P.Category

            query = spec.Includes.Aggregate(query, (currentQuery, queryExpression) => currentQuery.Include(queryExpression));

            // _dbContext.Set<Product>().Where( P => P.BrandId == 2 && true).OrderBy(P => P.Name).Include(P => P.Brand)
            // _dbContext.Set<Product>().Where( P => P.BrandId == 2 && true).OrderBy(P => P.Name).Include(P => P.Brand).Include(P => P.Category)




            return query;
        }

    }
}
