using BankingApp.Model;
using PRBD_Framework;

namespace BankingApp.ViewModel {
    public class ViewModelCommon : ViewModelBase<User, BankingContext> {
        public static bool IsManager => App.IsLoggedIn && App.CurrentUser.Role == Role.Manager;

        public static bool IsNotManager => !IsManager;

    }
}
