using BankingApp.Model;
using PRBD_Framework;
using System;
using System.Linq;
using System.Windows.Input;

namespace BankingApp.ViewModel {
    public class NewTransferDetailViewModel : ViewModelCommon {
        public NewTransferDetailViewModel() : base() {
            Accounts = new ObservableCollectionFast<Account>(ClientInternalAccount.GetIban(CurrentUser.UserId));
            AllCategories = new ObservableCollectionFast<Category>(Category.GetAll());
            Save = new RelayCommand(SaveAction, CanSaveAction);
            Cancel = new RelayCommand(CancelAction);
            DisplayAccounts = new RelayCommand<Account>(account => {
                App.ShowDialog<SelectAccountDialogViewModel, User, BankingContext>(account);
            });
            Register<Account>(App.Messages.MSG_ACCOUNT_SELECTED, account => {
                DestAccountString = account.Iban;
                RaisePropertyChanged();
            });
        }

        public ObservableCollectionFast<Account> Accounts { get; set; }
        public ObservableCollectionFast<Category> AllCategories { get; set; }
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand DisplayAccounts { get; set; }

        private Transfer _transfer;
        public Transfer Transfer {
            get => _transfer;
            set => SetProperty(ref _transfer, value);
        }

        private InternalAccount _originAccount;
        public InternalAccount OriginAccount {
            get => _originAccount;
            set {
                SetProperty(ref _originAccount, value, RaisePropertyChanged);
            }
        }

        private string _destAccountString;
        public string DestAccountString {
            get => _destAccountString;
            set => SetProperty(ref _destAccountString, value, () => Validate());
        }

        private Account _destAccount;
        public Account DestAccount {
            get => _destAccount;
            set => SetProperty(ref _destAccount, value, () => Validate());
        }
        private string _amount;
        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => Validate());
        }
        private string _description;
        public string Description {
            get => _description;
            set => SetProperty(ref _description, value, () => Validate());
        }

        private DateTime? _selectedEffect;

        public DateTime? SelectedEffectDate {
            get => _selectedEffect;
            set => SetProperty(ref _selectedEffect, value, () => Validate());
        }

        private Category _selectCategory;
        public Category SelectedCategory {
            get => _selectCategory;
            set => SetProperty(ref _selectCategory, value, () => Validate());
        }
        private string _categorieName;
        public string CategorieName {
            get => _categorieName;
            set => SetProperty(ref _categorieName, value, () => Validate());
        }
        private double AllowedAmount => OriginAccount.Balance - OriginAccount.Floor;
        public override void SaveAction() {
            DestAccount = Context.Accounts.SingleOrDefault(a => a.Iban == DestAccountString);
            Transfer = new Transfer(CurrentUser, Double.Parse(Amount), OriginAccount, DestAccount, Description, App.CurrentDate, SelectedEffectDate, SelectedCategory);
            Context.Transfers.Add(Transfer);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_ADD_TRANSFER);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB);
        }
        private bool CanSaveAction() {
            return !string.IsNullOrEmpty(Amount) && !string.IsNullOrEmpty(Description) && OriginAccount != null &&
                   !string.IsNullOrEmpty(DestAccountString) &&
                   (OriginAccount.isBalanceSufficient(Double.Parse(Amount)) || (SelectedEffectDate != null && SelectedEffectDate >= App.CurrentDate));
        }
        public override void CancelAction() {
            ClearErrors();
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB);
        }

        public override bool Validate() {
            string currentDateString = App.CurrentDate.ToString("dd/MM/yyyy");
            var categories = Context.Categories.Select(c => c.Name);
            ClearErrors();
            if (string.IsNullOrEmpty(DestAccountString))
                AddError(nameof(DestAccountString), "required");

            if (string.IsNullOrEmpty(Amount)) {
                AddError(nameof(Amount), "required");
            } else if (OriginAccount != null && !OriginAccount.isBalanceSufficient(Double.Parse(Amount)) && SelectedEffectDate == null) {
                AddError(nameof(Amount), $"Maximum allowed transfer is {AllowedAmount}€ ");
            }
            if (SelectedEffectDate < App.CurrentDate)
                AddError(nameof(SelectedEffectDate), $"must be between {App.CurrentDate.AddDays(1).ToString("dd-MM-yy")} and {App.CurrentDate.AddMonths(3).ToString("dd-MM-yy")}");
            if (string.IsNullOrEmpty(Description)) {
                AddError(nameof(Description), "required");
            }
            if (!string.IsNullOrEmpty(CategorieName) && !categories.Contains(CategorieName))
                AddError(nameof(CategorieName), "unknown category");
            return !HasErrors;
        }
    }
}
