using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BankingApp.Model {

    public class Transfer : EntityBase<BankingContext> {
        public Transfer() { }
        public Transfer(User user, double amount, Account originAccount, Account destinationAccount, string description, DateTime creationDate, DateTime? effectiveDate = null, Category category = null) {
            User = user;
            Amount = amount;
            OriginAccount = originAccount;
            DestinationAccount = destinationAccount;
            Description = description;
            CreationDate = creationDate;
            EffectiveDate = effectiveDate;
            Category = category;
            ConsolidatedDate = EffectiveDate ?? CreationDate;
            if (EffectiveDate != null)
                IsEffectDateDifferentCreationDate = EffectiveDate != CreationDate;
        }

        public int TransferId { get; set; }
        public int? UserId { get; set; }
        public double Amount { get; set; }
        public virtual User User { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int DestinationAccountId { get; set; }
        [Required]
        public virtual Account DestinationAccount { get; set; }
        public int OriginAccountId { get; set; }
        [Required]
        public virtual Account OriginAccount { get; set; }
        public string Description { get; set; }


        [NotMapped]
        public double AmountCopy { get; set; }
        [NotMapped]
        public DateTime ConsolidatedDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime CreationDate { get; set; }
        [NotMapped]
        public double BalanceTransfer { get; set; }
        [NotMapped]
        public bool HasEffectiveDateValue { get; set; }
        [NotMapped]
        public bool IsBalanceVisible { get; set; }
        [NotMapped]
        public bool IsTransferAccepted { get; set; }
        [NotMapped]
        public bool IsTransferRefusedAndBeforeCurrentDate { get; set; }
        [NotMapped]
        public bool IsEffectDateDifferentCreationDate { get; set; }
        public static List<Transfer> GetFiltered(string Filter, int accountId) {
            var filtered = Context.Transfers.AsEnumerable()
                           .Where(t => (t.OriginAccount.AccountId == accountId || t.DestinationAccount.AccountId == accountId) && t.CreationDate <= App.CurrentDate &&
                                      (t.User != null && t.User.FirstName.Contains(Filter) ||
                                       t.Description.Contains(Filter) || t.OriginAccount.Description.Contains(Filter) ||
                                       t.DestinationAccount.Description.Contains(Filter) || t.OriginAccount.Iban.Contains(Filter) ||
                                       t.DestinationAccount.Iban.Contains(Filter) ||
                                       t.BalanceTransfer.ToString().Contains(Filter) ||
                                       t.Amount.ToString().Contains(Filter)))
                           .OrderByDescending(t => t.ConsolidatedDate)
                           .ToList();
            return filtered;
        }
    }
}
