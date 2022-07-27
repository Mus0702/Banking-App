using BankingApp.Model;
using System;
using System.Linq;
using System.Net.Mail;

namespace BankingApp.ViewModel {
    public class ManagerClientDataViewModel : ViewModelCommon {
        private string EmailCopy { get; set; }
        private Client _client;
        public Client Client {
            get => _client;
            set {
                SetProperty(ref _client, value, RaisePropertyChanged);
                RaisePropertyChanged();
                EditMode = true;
                EmailCopy = Email;
                ConfirmPassword = Password;
            }
        }

        public string FirstName {
            get => Client?.FirstName;
            set => SetProperty(FirstName, value, Client, (s, v) => {
                s.FirstName = value;
                Validate();
                RaisePropertyChanged();
            });
        }
        public string LastName {
            get => Client?.LastName;
            set => SetProperty(LastName, value, Client, (s, v) => {
                s.LastName = value;
                Validate();
            });
        }

        public string Email {
            get => Client?.Email;
            set => SetProperty(Email, value, Client, (s, v) => {
                s.Email = value;
                Validate();
            });
        }

        public string Password {
            get => Client?.Password;
            set => SetProperty(Password, value, Client, (s, v) => {
                s.Password = value;
                Validate();
            });

        }
        private string _confirmPassword;
        public string ConfirmPassword {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value, () => Validate());
        }


        private bool _editMode = false;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }
        public override bool Validate() {
            var user = Context.Users.SingleOrDefault(user => user.Email == Email);
            var emails = Context.Users.Select(u => u.Email);
            ClearErrors();
            if (string.IsNullOrWhiteSpace(LastName))
                AddError(nameof(LastName), "required");
            if (string.IsNullOrWhiteSpace(FirstName))
                AddError(nameof(FirstName), "required");
            if (string.IsNullOrWhiteSpace(Email))
                AddError(nameof(Email), "required");
            else if (!IsValid(Email))
                AddError(nameof(Email), "Incorrect format");
            else if (EmailCopy != null && !EmailCopy.Equals(Email) && user != null || (EmailCopy == null && emails.Contains(Email)))
                AddError(nameof(Email), "Email is already assigned");
            if (string.IsNullOrWhiteSpace(Password))
                AddError(nameof(Password), "required");
            else if (Password.Length < 3)
                AddError(nameof(Password), "length must be >= 3");
            if (ConfirmPassword != Password)
                AddError(nameof(ConfirmPassword), "Passwords do not match");
            return !HasErrors;
        }
        public bool IsValid(string emailaddress) {
            try {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            } catch (FormatException) {
                return false;
            }
        }

    }
}
