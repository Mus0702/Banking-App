namespace BankingApp.Model {
    public class Admin : User {
        protected Admin() { }
        public Admin(string lastName, string firstName, string email, string password)
            : base(lastName, firstName, email, password) {
            Role = Role.Admin;
        }
    }
}
