namespace BankingApp.Model {
    public class CurrentAccount : InternalAccount {
        protected CurrentAccount() {
            AccountType = AccountType.CurrentAccount;
        }

        public CurrentAccount(string iban, string description, double floor) : base(iban, description, floor) {
            AccountType = AccountType.CurrentAccount;

        }
    }
}



