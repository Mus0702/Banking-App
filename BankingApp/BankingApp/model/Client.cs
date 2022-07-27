using System.Collections.Generic;

namespace BankingApp.Model {
    public class Client : User {
        public Client() { }
        public Client(string lastName, string firstName, string email, string password, Agency agency)
           : base(lastName, firstName, email, password) {
            Agency = agency;
            Role = Role.Client;
        }
        public virtual Agency Agency { get; set; }
        public virtual ICollection<ClientInternalAccount> ListClientInternalAccount { get; set; } = new HashSet<ClientInternalAccount>();

        public void Delete() {
            Context.Clients.Remove(this);
            Context.SaveChanges();
        }
    }
}
