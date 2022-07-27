using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BankingApp.Model;
using PRBD_Framework;
namespace BankingApp.ViewModel {
    class SelectAccountDialogViewModel : DialogViewModelBase<User, BankingContext> {

        private ObservableCollectionFast<Account> _myAccounts;
       
        public ObservableCollectionFast<Account> MyAccounts {
            get => _myAccounts;
            set => SetProperty(ref _myAccounts, value);
        }
        private ObservableCollectionFast<Account> _otherAccounts;
        public ObservableCollectionFast<Account> OtherAccounts {
            get => _otherAccounts;
            set => SetProperty(ref _otherAccounts, value);
        }
        private IQueryable<string> MyAccountsIban { get; set; }
        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }
        private Account _exludedAccount;
        public Account ExcludedAccount {
            get => _exludedAccount;
            set => SetProperty(ref _exludedAccount, value);
        }
        private bool _isEnable = false;
        public bool IsEnable {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }
        Account _selectedAccount;
        public Account SelectedAccount {
            get => _selectedAccount;
            set {
                _selectedAccount = value;
                IsEnable = true;
                NotifyColleagues(App.Messages.MSG_ACCOUNT_SELECTED, value);
                Console.WriteLine(value);
            }
        }
        public ICommand CloseWindow { get; set; }
        public void init() {
            MyAccounts = new ObservableCollectionFast<Account>(ClientInternalAccount.GetMyAccounts(CurrentUser.UserId, ExcludedAccount.AccountId));
            // var myAccounts= Context.InternalAccounts.Where(a=>a.)
            MyAccountsIban = Context.ClientsInternalAccounts.Where(c => c.ClientId == CurrentUser.UserId && c.InternalAccount != ExcludedAccount).Select(a => a.InternalAccount.Iban);
            //  var otherAccounts = Context.Accounts.Where(a => !myAccounts.Contains(a.Iban) && a.AccountId !=ExcludedAccount.AccountId);
                OtherAccounts = new ObservableCollectionFast<Account>(Account.GetOtherAccounts(MyAccountsIban, ExcludedAccount));
            if (ExcludedAccount.AccountType == AccountType.SavingAccount)
                OtherAccounts.Clear();



        }


        public SelectAccountDialogViewModel() {
            // OtherAccounts = new ObservableCollectionFast<ClientInternalAccount>(ClientInternalAccount.GetOtherAccounts(CurrentUser.UserId));
            CloseWindow = new RelayCommand(() => DialogResult = false);
        }
        protected override void OnRefreshData() {
            var filteredMyAccount = string.IsNullOrEmpty(Filter) ? Model.ClientInternalAccount.GetMyAccounts(CurrentUser.UserId, ExcludedAccount.AccountId) : Model.ClientInternalAccount.GetFilteredMyAccount(Filter, CurrentUser.UserId, ExcludedAccount);
            MyAccounts = new ObservableCollectionFast<Account>(filteredMyAccount);
            var filteredOtherAccount = string.IsNullOrEmpty(Filter) ? Account.GetOtherAccounts(MyAccountsIban, ExcludedAccount) : Model.Account.GetFiltered(Filter, MyAccountsIban, ExcludedAccount);
            OtherAccounts = new ObservableCollectionFast<Account>(filteredOtherAccount);
            if (ExcludedAccount.AccountType == AccountType.SavingAccount)
                OtherAccounts.Clear();

            //RaisePropertyChanged(nameof(MyAccounts));
            //RaisePropertyChanged(nameof(OtherAccounts));
        }


    }
}
