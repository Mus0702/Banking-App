using BankingApp.Model;
using PRBD_Framework;
using System;
using System.Linq;
using System.Windows.Input;

namespace BankingApp.ViewModel {
    public class AccountsViewModel : ViewModelCommon {
        public AccountsViewModel() : base() {
            Register(App.Messages.MSG_CURRENT_DATE_CHANGED, () => OnRefreshData());
            Register(App.Messages.MSG_ADD_TRANSFER, OnRefreshData);

            ClientInternalAccount = new ObservableCollectionFast<ClientInternalAccount>(Model.ClientInternalAccount.GetAll(CurrentUser.UserId));
            ClearFilter = new RelayCommand(() => Filter = "");
            NewTransfer = new RelayCommand<ClientInternalAccount>(account => NotifyColleagues(App.Messages.MSG_NEW_TRANSFER, account));
            ShowStatements = new RelayCommand<ClientInternalAccount>(account => NotifyColleagues(App.Messages.MSG_SHOW_STATEMENTS, account));
            All = true;
        }

        private ObservableCollectionFast<ClientInternalAccount> _clientInternalAccount;
        public ObservableCollectionFast<ClientInternalAccount> ClientInternalAccount {
            get => _clientInternalAccount;
            set => SetProperty(ref _clientInternalAccount, value);
        }

        private bool _checking;
        public bool Checking {
            get => _checking;
            set => SetProperty(ref _checking, value, OnRefreshData);
        }

        private bool _savings;
        public bool Savings {
            get => _savings;
            set => SetProperty(ref _savings, value, OnRefreshData);
        }

        private bool _all;
        public bool All {
            get => _all;
            set => SetProperty(ref _all, value, OnRefreshData);
        }
        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }
        public ICommand ClearFilter { get; set; }
        public ICommand NewTransfer { get; set; }
        public ICommand ShowStatements { get; set; }

        protected override void OnRefreshData() {
            InternalAccount.getBalance();
            if (_checking && _savings ||
                _checking && _all ||
                _all && _savings)
                return;
            IQueryable<ClientInternalAccount> accounts = string.IsNullOrEmpty(Filter) ? Model.ClientInternalAccount.GetAll(CurrentUser.UserId) : Model.ClientInternalAccount.GetFiltered(Filter, CurrentUser.UserId);
            var filteredAccounts = from c in accounts
                                   where Checking && c.InternalAccount is CurrentAccount ||
                                   Savings && c.InternalAccount is SavingAccount ||
                                   All
                                   select c;
            ClientInternalAccount = new ObservableCollectionFast<ClientInternalAccount>(filteredAccounts);
            RaisePropertyChanged(nameof(ClientInternalAccount));

        }
    }
}
