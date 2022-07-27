using PRBD_Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace BankingApp.Model {
    public class Agency : EntityBase<BankingContext> {
        protected Agency() { }
        public Agency(string name, Manager manager) {
            Name = name;
            Manager = manager;
        }
        public int AgencyId { get; set; }
        public string Name { get; set; }

        public int ManagerId { get; set; }
        [Required]
        public virtual Manager Manager { get; set; }

        public virtual ICollection<Client> ListClients { get; set; } = new HashSet<Client>();

        public static IQueryable<Agency> GetAllAgencies(int userId) {
            return Context.Agencies.Where(a => a.ManagerId == userId);
        }
    }
}
