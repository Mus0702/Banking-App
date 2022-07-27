using PRBD_Framework;
using System.Windows;

namespace BankingApp.View {
    public partial class LoginView : WindowBase {
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        private void txtEmail_GotFocus(object sender, RoutedEventArgs e) {
            txtEmail.SelectAll();
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e) {
            txtPassword.SelectAll();
        }
        public LoginView() {
            InitializeComponent();
        }
    }
}
