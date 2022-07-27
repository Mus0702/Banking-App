using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingApp.Model {
    public abstract class InternalAccount : Account {
        protected InternalAccount() { }
        public InternalAccount(string iban, string description, double floor) : base(iban, description, floor) {
        }
        public virtual ICollection<ClientInternalAccount> ListClientInternalAccount { get; set; } = new HashSet<ClientInternalAccount>();


        public void GetBalanceTransfers() {
            var transfers = Context.Transfers.AsEnumerable()
                .Where(t => (t.OriginAccount.AccountId == AccountId || t.DestinationAccount.AccountId == AccountId) && t.ConsolidatedDate <= App.CurrentDate)
                .OrderBy(d => d.ConsolidatedDate)
                .ToList();
            BalanceAfterTransfer = 0;
            foreach (var t in transfers) {
                if (t.OriginAccount is ExternalAccount || t.OriginAccount.isBalanceAfterTransferSufficient(t.Amount)) {
                    if (t.IsTransferAccepted) {
                        t.OriginAccount.BalanceAfterTransfer -= t.Amount;
                        t.DestinationAccount.BalanceAfterTransfer += t.Amount;
                    }
                    t.BalanceTransfer = BalanceAfterTransfer;
                }
            }
        }
        public static void getBalance() {
            var AllTransfers = Context.Transfers
                .AsEnumerable()
                .Where(t => t.ConsolidatedDate <= App.CurrentDate)
                .OrderBy(t => t.ConsolidatedDate)
                .ToList();
            var allAccounts = Context.InternalAccounts;
            foreach (var a in allAccounts) {
                a.Balance = 0;
                a.BalanceAfterTransfer = 0;
            }
            foreach (var t in AllTransfers) {
                if (t.OriginAccount is ExternalAccount || t.OriginAccount.isBalanceSufficient(t.Amount)) {
                    t.IsTransferAccepted = true;
                    if (t.OriginAccount is not ExternalAccount) {
                        t.BalanceTransfer -= t.Amount;
                        t.OriginAccount.Balance -= t.Amount;
                    }
                    t.DestinationAccount.BalanceAfterTransfer += t.Amount;
                    t.DestinationAccount.Balance += t.Amount;
                } else if (t.OriginAccount is not ExternalAccount && !t.OriginAccount.isBalanceSufficient(t.Amount)) {
                    t.IsTransferAccepted = false;
                }
            }
        }
        public List<Transfer> GetTransfers() {
            var transfers = Context.Transfers.AsEnumerable()
                .Where(t => (t.OriginAccount.AccountId == AccountId || t.DestinationAccount.AccountId == AccountId) && t.CreationDate <= App.CurrentDate).
                OrderByDescending(d => d.ConsolidatedDate)
                .ToList();
            foreach (var t in transfers) {
                t.HasEffectiveDateValue = t.EffectiveDate != t.CreationDate && t.EffectiveDate > App.CurrentDate;
                t.IsBalanceVisible = t.IsTransferAccepted && t.ConsolidatedDate <= App.CurrentDate;
                t.IsTransferRefusedAndBeforeCurrentDate = !t.IsTransferAccepted && t.ConsolidatedDate <= App.CurrentDate;

            }
            InternalAccount.getBalance();
            return transfers;
        }
    }
}
