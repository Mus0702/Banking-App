using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BankingApp.Model {
    public enum TypeClient {
        Holder = 1,
        Mandatory = 2
    }
    public class ClientInternalAccount : EntityBase<BankingContext> {
        public ClientInternalAccount() { }
        public ClientInternalAccount(Client client, InternalAccount internalAccount, TypeClient typeClient) {
            Client = client;
            InternalAccount = internalAccount;
            TypeCLient = typeClient;
        }

        public int ClientId { get; set; }
        [Required]
        public virtual Client Client { get; set; }
        public int InternalAccountId { get; set; }
        [Required]
        public virtual InternalAccount InternalAccount { get; set; }
        public TypeClient TypeCLient { get; protected set; }

        public static IQueryable<ClientInternalAccount> GetFiltered(string Filter, int userId) {
            var filtered = from c in Context.ClientsInternalAccounts
                           where (c.InternalAccount.Iban.Contains(Filter) || c.InternalAccount.Description.Contains(Filter)) && c.ClientId == userId select c;
            return filtered;
        }
        public static IQueryable<Account> GetFilteredMyAccount(string Filter, int userId, Account ExcludedAccount) {
            var filtered = from c in Context.ClientsInternalAccounts
                           where (c.ClientId == userId && c.InternalAccount.AccountId != ExcludedAccount.AccountId &&
                           (c.InternalAccount.Iban.Contains(Filter) || c.InternalAccount.Description.Contains(Filter)))
                           select c.InternalAccount;
            return filtered;
        }
        public static IQueryable<InternalAccount> GetIban(int userId) {
            var ibans = from c in Context.ClientsInternalAccounts
                        where c.ClientId == userId select c.InternalAccount;
            return ibans;
        }
        public static IQueryable<ClientInternalAccount> GetAll(int userId) {
            var q = Context.ClientsInternalAccounts.Where(c => c.ClientId == userId);
            return q;
        }

        public static IQueryable<Account> GetMyAccounts(int userId, int accountId) {
            return Context.ClientsInternalAccounts.Where(c => c.ClientId == userId && c.InternalAccountId != accountId).Select(a => a.InternalAccount);
        }
    }
}
