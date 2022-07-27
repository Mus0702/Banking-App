using BankingApp.Model;
using PRBD_Framework;
namespace BankingApp.View {
    public partial class SelectAccountDialogView : DialogWindowBase {
        public SelectAccountDialogView(Account account) {
            InitializeComponent();
            vm.ExcludedAccount = account;
            vm.init();
        }
        private void DialogWindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (vm.DialogResult == null)
                e.Cancel = true;
        }
    }
}
