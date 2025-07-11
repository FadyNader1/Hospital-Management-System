using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Specifications
{
    public class BaseSpecification<T>:ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludeStrings { get; set; }

        public BaseSpecification()
        {
            Includes = new List<Expression<Func<T, object>>>();
            IncludeStrings = new List<string>();
        }

        public BaseSpecification(Expression<Func<T,bool>> Criteria)
        {
            Includes = new List<Expression<Func<T, object>>>();
            IncludeStrings = new List<string>();
            this.Criteria = Criteria;
        }

        public void AddInclude(string incstring)
        {
            IncludeStrings.Add(incstring);
        }

    
    }
}
