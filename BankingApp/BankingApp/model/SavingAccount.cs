namespace BankingApp.Model {
    public class SavingAccount : InternalAccount {
        protected SavingAccount() {
            AccountType = AccountType.SavingAccount;
        }
        public SavingAccount(string iban, string description, double floor = 0) : base(iban, description, floor) {
            AccountType = AccountType.SavingAccount;
        }
    }
}

