using BankingApp.Model;
using PRBD_Framework;
using System.Windows;

namespace BankingApp.View {
    public partial class NewTransferDetailView : UserControlBase {
        public NewTransferDetailView(ClientInternalAccount account) {
            InitializeComponent();
            vm.OriginAccount = account.InternalAccount;
        }

        private void txtAmount_GotFocus(object sender, RoutedEventArgs e) {
            txtAmount.SelectAll();
        }

        private void txtDescription_GotFocus(object sender, RoutedEventArgs e) {
            txtDescription.SelectAll();
        }
    }

}
