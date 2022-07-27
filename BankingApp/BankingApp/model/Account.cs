using Microsoft.EntityFrameworkCore;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BankingApp.Model {
    public enum AccountType {
        CurrentAccount = 1,
        SavingAccount = 2,
        ExternalAccount = 3
    }
    public abstract class Account : EntityBase<BankingContext> {
        protected Account() { }
        public Account(string iban, string description, double floor) {
            Iban = iban;
            Description = description;
            Floor = floor;
        }
        public int AccountId { get; set; }
        public string Iban { get; set; }
        public string Description { get; set; }
        public double Floor { get; set; }
        public AccountType AccountType { get; protected set; }
        [NotMapped]
        public double Balance { get; set; }
        [NotMapped]
        public double BalanceAfterTransfer { get; set; }
        public virtual ICollection<Transfer> ListReceivedTransfers { get; set; } = new HashSet<Transfer>();

        public virtual ICollection<Transfer> ListSentTransfers { get; set; } = new HashSet<Transfer>();
        public bool isBalanceSufficient(double amount) {
            return amount <= Balance - Floor;

        }
        public bool isBalanceAfterTransferSufficient(double amount) {
            return amount <= BalanceAfterTransfer - Floor;

        }
        public static IQueryable<Account> GetOtherAccounts(IQueryable<string> myAccounts, Account ExcludedAccount) {
            return Context.Accounts
                          .Where(a => !myAccounts.Contains(a.Iban) && a.AccountId != ExcludedAccount.AccountId);
        }
        public static IQueryable<Account> GetFiltered(string Filter, IQueryable<string> myAccounts, Account ExcludedAccount) {
            var filtered = from a in Context.Accounts
                           where (!myAccounts.Contains(a.Iban) && a.AccountId != ExcludedAccount.AccountId && (a.Iban.Contains(Filter) || a.Description.Contains(Filter)))
                           select a;
            return filtered;
        }
    }
}
