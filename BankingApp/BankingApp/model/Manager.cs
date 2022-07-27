using System.Collections.Generic;

namespace BankingApp.Model {
    public class Manager : User {
        public Manager() { }
        public Manager(string lastName, string firstName, string email, string password)
           : base(lastName, firstName, email, password) {
            Role = Role.Manager;
        }
        public virtual ICollection<Agency> ListAgencies { get; set; } = new HashSet<Agency>();
    }
}
