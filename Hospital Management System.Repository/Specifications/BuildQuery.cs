using Hospital_Management_System.Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Repository.Specifications
{
    public class BuildQuery<T>:BaseSpecification<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> basepart,ISpecification<T> spec)
        {
            var query = basepart;
            // Apply the criteria if it exists
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);
            // Apply the includes if they exist
            if (spec.IncludeStrings != null && spec.IncludeStrings.Any())
                foreach (var includeString in spec.IncludeStrings)
                    query = query.Include(includeString);

            // Apply the includes using the expression trees if they exist
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
