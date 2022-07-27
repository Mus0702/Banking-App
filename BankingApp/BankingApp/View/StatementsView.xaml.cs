using BankingApp.Model;
using PRBD_Framework;
namespace BankingApp.View {
    public partial class StatementsView : UserControlBase {
        public StatementsView(ClientInternalAccount account) {
            InitializeComponent();
            vm.Init(account);
        }
    }
}
