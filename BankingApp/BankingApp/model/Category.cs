using PRBD_Framework;
using System.Collections.Generic;
using System.Linq;

namespace BankingApp.Model {
    public class Category : EntityBase<BankingContext> {
        protected Category() { }
        public Category(string name) : base() {
            Name = name;
        }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Transfer> ListTransfers { get; set; } = new HashSet<Transfer>();
        public static IQueryable<Category> GetAll() {
            return Context.Categories.OrderBy(c => c.Name);
        }
    }
}
