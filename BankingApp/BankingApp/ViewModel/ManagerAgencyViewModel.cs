using BankingApp.Model;
using PRBD_Framework;
using System;
using System.Linq;
using System.Windows.Input;

namespace BankingApp.ViewModel {
    public class ManagerAgencyViewModel : ViewModelCommon {
        public ManagerClientDataViewModel ManagerClientData { get; private set; } = new ManagerClientDataViewModel();
        public ManagerAgencyViewModel() {
            Register(App.Messages.MSG_CURRENT_DATE_CHANGED, () => AccountsOfCLient());
            OnRefreshData();
            Save = new RelayCommand(SaveAction, CanSaveAction);
            Cancel = new RelayCommand(CancelAction);
            Delete = new RelayCommand(DeleteAction, () => IsDeletable);
            NewClient = new RelayCommand(NewClientAction, () => IsSavedAndValid);
            AccountDetailsShow = new RelayCommand<ClientInternalAccount>(account => NotifyColleagues(App.Messages.MSG_SHOW_STATEMENTS, account));
            AddAccess = new RelayCommand(AddAccessToClient, () => FreeIban != null);
        }
        public ICommand NewClient { get; set; }
        public ICommand Save { get; set; }
        public ICommand Delete { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand AccountDetailsShow { get; set; }
        public ICommand AddAccess { get; set; }
        public ObservableCollectionFast<Agency> Agencies { get; set; } = new();
        public ObservableCollectionFast<Client> Clients { get; set; } = new();
        public ObservableCollectionFast<ClientInternalAccount> Accounts { get; set; } = new();
        public ObservableCollectionFast<Account> FreeAccounts { get; set; } = new();
        private ClientInternalAccount _newAccess;
        public ClientInternalAccount NewAccess {
            get => _newAccess;
            set => SetProperty(ref _newAccess, value);
        }

        public InternalAccount _freeIban;
        public InternalAccount FreeIban {
            get => _freeIban;
            set => SetProperty(ref _freeIban, value);
        }
        private TypeClient _selectedTypeClient = TypeClient.Holder;
        public TypeClient SelectedTypeCLient {
            get => _selectedTypeClient;
            set => SetProperty(ref _selectedTypeClient, value);
        }
        private TypeClient[] _typeClient;
        public TypeClient[] SelectableAccess {
            get {
                return _typeClient ??
                       (_typeClient = Enum.GetValues(typeof(TypeClient)).Cast<TypeClient>().ToArray());
            }
        }

        private bool _isNew;
        public bool IsNew {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }
        public bool IsSavedAndValid => !IsNew && !HasChanges;
        public bool IsDeletable => !IsNew && SelectedClient != null;
        private Client _selectedClient;
        public Client SelectedClient {
            get => _selectedClient;
            set {
                SetProperty(ref _selectedClient, value, () => ManagerClientData.Client = value);
                if (SelectedClient != null) {
                    AccountsOfCLient();
                    FreeAcountsClient();
                }
            }
        }
        private Agency _selectedAgency;
        public Agency SelectedAgency {
            get => _selectedAgency;
            set => SetProperty(ref _selectedAgency, value, () => ClientOfAgency(SelectedAgency));
        }

        private void ClientOfAgency(Agency agency) {
            if (agency != null) {
                Clients.RefreshFromModel(Context.Clients.Where(c => c.Agency.AgencyId == agency.AgencyId));
            }
        }
        private void AccountsOfCLient() {
            if (SelectedClient != null)
                Accounts.RefreshFromModel(Context.ClientsInternalAccounts.Where(a => a.ClientId == SelectedClient.UserId));
            else
                Accounts.Clear();
        }
        private void FreeAcountsClient() {
            var accounts = Context.ClientsInternalAccounts.Where(a => a.ClientId == SelectedClient.UserId).Select(a => a.InternalAccount.Iban);
            var otherAccounts = Context.InternalAccounts.Where(a => !accounts.Contains(a.Iban));

            FreeAccounts = new ObservableCollectionFast<Account>(otherAccounts);
            RaisePropertyChanged(nameof(FreeAccounts));


        }
        protected override void OnRefreshData() {
            Agencies.RefreshFromModel(Context.Agencies.Where(a => a.ManagerId == CurrentUser.UserId).OrderBy(a => a.Name));
            ClientOfAgency(SelectedAgency);
        }

        private void NewClientAction() {
            if (_selectedAgency != null) {
                SelectedClient = new Client();
                IsNew = true;
                Clients.Add(SelectedClient);
                RaisePropertyChanged(nameof(SelectedClient));
                RaisePropertyChanged(nameof(Clients));
            }
        }

        public override void SaveAction() {
            if (IsNew) {
                SelectedClient.Agency = SelectedAgency;
                Context.Add(SelectedClient);
                IsNew = false;
                Context.SaveChanges();
            }
            if (NewAccess != null && Context.Entry(NewAccess).State == Microsoft.EntityFrameworkCore.EntityState.Added) {
                Context.SaveChanges();
                SelectedClient = null;
                AccountsOfCLient();
            }
            if (ManagerClientData.Validate()) {
                Context.SaveChanges();
                SelectedClient = null;
                ManagerClientData.EditMode = false;
            }
            OnRefreshData();
        }

        private bool CanSaveAction() {
            return IsNew || SelectedClient != null;
        }
        private void DeleteAction() {
            SelectedClient.Delete();
            ClientOfAgency(SelectedAgency);
            AccountsOfCLient();
            ManagerClientData.EditMode = false;
        }

        private void AddAccessToClient() {
            NewAccess = new ClientInternalAccount(SelectedClient, FreeIban, SelectedTypeCLient);
            Context.ClientsInternalAccounts.Add(NewAccess);
            Accounts.Add(NewAccess);
            FreeIban = null;
            FreeAcountsClient();
            RaisePropertyChanged(nameof(Accounts));
        }

        public override void CancelAction() {

            if (IsNew) {
                Clients.Remove(SelectedClient);
                IsNew = false;
                ManagerClientData.EditMode = false;
                RaisePropertyChanged();

            } else if (!IsNew && SelectedClient != null) {
                SelectedClient.Reload();
            }
            if (Context.Entry(NewAccess).State == Microsoft.EntityFrameworkCore.EntityState.Added) {
                Context.ClientsInternalAccounts.Remove(NewAccess);
                Accounts.Remove(NewAccess);
                RaisePropertyChanged(nameof(Accounts));
            }
            ClearErrors();
        }
    }
}
