namespace BankingApp.Model {
    public class ExternalAccount : Account {
        protected ExternalAccount() {
            AccountType = AccountType.ExternalAccount;
        }
        public ExternalAccount(string iban, string description, double floor = 0) : base(iban, description, floor) {
            AccountType = AccountType.ExternalAccount;
        }
    }
}
