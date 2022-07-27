using PRBD_Framework;
using System.Windows;

namespace BankingApp.View {
    public partial class ManagerClientDataView : UserControlBase {
        public ManagerClientDataView() {
            InitializeComponent();
        }
        private void txtFirstName_GotFocus(object sender, RoutedEventArgs e) {
            txtFirstName.SelectAll();
        }
        private void txtLastName_GotFocus(object sender, RoutedEventArgs e) {
            txtLastName.SelectAll();
        }
        private void txtEmail_GotFocus(object sender, RoutedEventArgs e) {
            txtEmail.SelectAll();
        }
        private void txtPassword_GotFocus(object sender, RoutedEventArgs e) {
            txtPassword.SelectAll();
        }
        private void txtPasswordConfirm_GotFocus(object sender, RoutedEventArgs e) {
            txtPasswordConfirm.SelectAll();
        }
    }
}
