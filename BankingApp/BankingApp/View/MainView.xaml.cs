using BankingApp.Model;
using PRBD_Framework;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace BankingApp.View {
    public partial class MainView : WindowBase {
        public MainView() {
            InitializeComponent();
            Register<ClientInternalAccount>(App.Messages.MSG_NEW_TRANSFER, account => OpenTab("New Transfer", "New Transfer", () => new NewTransferDetailView(account)));
            Register<ClientInternalAccount>(App.Messages.MSG_SHOW_STATEMENTS, account => DoDisplayTransfer(account));
        }
        private void DoDisplayTransfer(ClientInternalAccount account) {
            if (account != null) {
                OpenTab(account.InternalAccount.Iban, account.InternalAccount.Iban, () => new StatementsView(account));
            }
        }

        private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
            var tab = tabControl.FindByTag(tag);
            if (tab == null)
                tabControl.Add(createView(), header, tag);
            else
                tabControl.SetFocus(tab);
        }

        private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }

        private void WindowBase_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl))
                Close();
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            tabControl.Dispose();
        }
        private void Vm_CloseTab() {
            var tab = tabControl.FindByTag("New Transfer");
            tabControl.Items.Remove(tab);
        }
    }
}

