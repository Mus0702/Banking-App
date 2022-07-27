using BankingApp.Model;
using BankingApp.Properties;
using BankingApp.ViewModel;
using PRBD_Framework;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace BankingApp {

    public partial class App : ApplicationBase<User, BankingContext> {
        public enum Messages {
            MSG_LOGIN,
            MSG_NEW_TRANSFER,
            MSG_SHOW_STATEMENTS,
            MSG_LOGOUT,
            MSG_ADD_TRANSFER,
            MSG_CURRENT_DATE_CHANGED,
            MSG_ACCOUNT_SELECTED,
            MSG_CLOSE_TAB,
            MSG_NEW_CLIENT,
            MSG_CATEGORY_FILTER_CHANGED
        }
        private static DateTime _currentDate = DateTime.Now;
        public static DateTime CurrentDate {
            get {
                return _currentDate;
            }
            set {
                _currentDate = value;
                InternalAccount.getBalance();
                NotifyColleagues(Messages.MSG_CURRENT_DATE_CHANGED);
            }
        }

        private void SetCulture() {
            var culture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .SingleOrDefault(c => c.IetfLanguageTag.Equals(Settings.Default.Locale, StringComparison.OrdinalIgnoreCase));
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        culture == null || culture.Name == "" ?
                            CultureInfo.CurrentCulture.IetfLanguageTag :
                            Settings.Default.Locale)));
            if (culture != null && culture.Name != "")
                CultureInfo.CurrentCulture = culture;
            else
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name);
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Console.OutputEncoding = Encoding.UTF8;
            SetCulture();
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Context.SeedData();

            Register<User>(this, Messages.MSG_LOGIN, user => {
                Login(user);
                NavigateTo<MainViewModel, User, BankingContext>();
            });

            Register(this, Messages.MSG_LOGOUT, () => {
                Logout();
                NavigateTo<LoginViewModel, User, BankingContext>();
            });
        }
        protected override void OnRefreshData() {
            if (CurrentUser?.UserId != null)
                CurrentUser = User.GetById(CurrentUser.UserId);
        }
    }
}

