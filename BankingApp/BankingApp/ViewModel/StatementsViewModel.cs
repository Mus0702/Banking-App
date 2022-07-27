using BankingApp.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;


namespace BankingApp.ViewModel {
    public class Period {
        public Period(int number, string type, TimeSpan duration) {
            Number = number;
            Type = type;
            Duration = duration;
        }
        public int Number;
        public string Type { get; set; }
        public TimeSpan Duration;
    }
    public class Categories : ViewModelCommon {
        public Categories(Category category, bool isChecked = true) {
            Category = category;

            IsChecked = isChecked;
        }
        public Category Category { get; set; }
        private bool _isChecked;
        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                NotifyColleagues(App.Messages.MSG_CATEGORY_FILTER_CHANGED);
                RaisePropertyChanged();
            }
        }
    }

    public class StatementsViewModel : ViewModelCommon {
        public StatementsViewModel() : base() {
            Register(App.Messages.MSG_CURRENT_DATE_CHANGED, OnRefreshData);
            Register(App.Messages.MSG_ADD_TRANSFER, OnRefreshData);
            Register(App.Messages.MSG_CATEGORY_FILTER_CHANGED, OnRefreshData);
            Periods = AddPeriodsToList(Periods);
            AllCategories = new ObservableCollectionFast<Category>(Category.GetAll());
            CheckAll = new RelayCommand(CheckAllCategory);
            CheckNone = new RelayCommand(CheckNoneCategory);
            SaveCategory = new RelayCommand<Transfer>(transfer => Context.SaveChanges());
            Cancel = new RelayCommand<Transfer>(transfer => CancelTransfer(transfer));
        }
        public ICommand SaveCategory { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand CheckAll { get; set; }
        public ICommand CheckNone { get; set; }

        private ClientInternalAccount _account;
        public ClientInternalAccount Account {
            get => _account;
            set => SetProperty(ref _account, value);
        }
        private List<Transfer> _listTransfers;
        public List<Transfer> ListTransfers {
            get => _listTransfers;
            set {
                _listTransfers = value;
            }
        }
        public ObservableCollectionFast<Category> AllCategories { get; set; }
        public ObservableCollectionFast<Categories> CategoriesToDisplay { get; set; } = new();
        public ObservableCollectionFast<Period> Periods { get; set; } = new();

        private void AddCategoriesToDisplay() {
            CategoriesToDisplay.Add(new Categories(new Category("<No Category>")));
            foreach (var c in AllCategories) {
                CategoriesToDisplay.Add(new Categories(c));
            }
        }

        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        private bool _isFutureSelected = false;
        public bool IsFutureSelected {
            get => _isFutureSelected;
            set => SetProperty(ref _isFutureSelected, value, OnRefreshData);
        }
        private bool _isPastSelected = true;
        public bool IsPastSelected {
            get => _isPastSelected;
            set => SetProperty(ref _isPastSelected, value, OnRefreshData);
        }
        private bool _isRefused = false;
        public bool IsRefused {
            get => _isRefused;
            set => SetProperty(ref _isRefused, value, OnRefreshData);
        }
        private Period _selectedPeriod;
        public Period SelectedPeriod {
            get => _selectedPeriod;
            set => SetProperty(ref _selectedPeriod, value, OnRefreshData);
        }
        public void Init(ClientInternalAccount account) {
            Account = account;
            SelectedPeriod = Periods[0];
            AddCategoriesToDisplay();
        }

        private ObservableCollectionFast<Period> AddPeriodsToList(ObservableCollectionFast<Period> Periods) {
            Periods.Add(new Period(0, "All", TimeSpan.FromDays(500)));
            Periods.Add(new Period(1, "1 day", TimeSpan.FromDays(1)));
            Periods.Add(new Period(1, "1 week", TimeSpan.FromDays(7)));
            Periods.Add(new Period(2, "2 weeks", TimeSpan.FromDays(14)));
            Periods.Add(new Period(4, "4 weeks", TimeSpan.FromDays(28)));
            Periods.Add(new Period(1, "1 year", TimeSpan.FromDays(365)));

            return Periods;
        }
        protected override void OnRefreshData() {
            List<Transfer> transfers = string.IsNullOrEmpty(Filter) ? Account.InternalAccount.GetTransfers() : Transfer.GetFiltered(Filter, Account.InternalAccountId);
            var filtered = transfers.AsEnumerable()
                            .Where(t => IsPastSelected && t.ConsolidatedDate <= App.CurrentDate && t.IsTransferAccepted ||
                                       IsFutureSelected && t.EffectiveDate > App.CurrentDate ||
                                       IsRefused && !t.IsTransferAccepted && t.ConsolidatedDate <= App.CurrentDate)
                           .Where(t => t.ConsolidatedDate > App.CurrentDate.Subtract(SelectedPeriod.Duration))
                           .Where(t => CategoriesToDisplay.Any(c => c.IsChecked && t.Category == c.Category ||
                                                                   (c.IsChecked && c.Category.Name == "<No Category>" && t.Category == null)))
                           .OrderByDescending(t => t.ConsolidatedDate)
                           .ToList();

            ListTransfers = new List<Transfer>(filtered);
            ChangeAmountSign();
            foreach (var t in ListTransfers.ToList()) {
                if (t.DestinationAccountId == Account.InternalAccountId && !t.IsTransferAccepted)
                    ListTransfers.Remove(t);
            }
            Account.InternalAccount.GetBalanceTransfers();
            RaisePropertyChanged();
        }
        private void CheckAllCategory() {
            foreach (var c in CategoriesToDisplay) {
                c.IsChecked = true;
            }
        }
        private void CheckNoneCategory() {
            foreach (var c in CategoriesToDisplay) {
                c.IsChecked = false;
            }
        }
        private void CancelTransfer(Transfer transfer) {
            Context.Transfers.Remove(transfer);
            Context.SaveChanges();
            OnRefreshData();
        }
        private void ChangeAmountSign() {
            foreach (var f in ListTransfers) {
                if (f.OriginAccountId == Account.InternalAccountId) {
                    f.AmountCopy = f.Amount * -1;
                } else if (f.DestinationAccount.Iban == Account.InternalAccount.Iban && f.AmountCopy < 0) {
                    f.AmountCopy = f.Amount;
                } else
                    f.AmountCopy = f.Amount;
            }
        }
    }
}
