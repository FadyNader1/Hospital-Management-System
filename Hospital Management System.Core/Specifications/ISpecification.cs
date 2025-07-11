using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Specifications
{
    public interface ISpecification<T>
    {
     public Expression<Func<T, bool>> Criteria { get; set; }
     public   List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludeStrings { get; set; }

    }
}
