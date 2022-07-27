using PRBD_Framework;
using System;
using System.Linq;
using System.Net.Mail;
using System.Windows.Input;

namespace BankingApp.ViewModel {
    class LoginViewModel : ViewModelCommon {
        public ICommand LoginCommand { get; set; }
        private string _email;
        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => Validate());
        }
        private string _password;
        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction, () => { return _email != null && _password != null && !HasErrors; });
        }
        private void LoginAction() {
            if (Validate()) {
                var user = Context.Users.SingleOrDefault(user => user.Email == _email);
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }

        public override bool Validate() {
            ClearErrors();
            var user = Context.Users.SingleOrDefault(user => user.Email == _email);

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (!IsValid(Email))
                AddError(nameof(Email), "Incorrect format");
            else if (user == null)
                AddError(nameof(Email), " doesn't exist");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 3)
                    AddError(nameof(Password), "length must be >= 3");
                else if (user != null && user.Password != Password)
                    AddError(nameof(Password), "wrong password");
            }
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
