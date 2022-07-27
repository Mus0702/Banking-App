using System;

namespace BankingApp.ViewModel {
    public class MainViewModel : ViewModelCommon {
        public event Action CloseTab;
        public MainViewModel() : base() {
            Info = "(" + CurrentUser.FirstName + " - " + CurrentUser.Role + ")";
            Register(App.Messages.MSG_CLOSE_TAB, () => CloseTab?.Invoke());
        }
        public string Info { get; set; }
    }
}
