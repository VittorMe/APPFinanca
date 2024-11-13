using APPFinanca.Repositories;
using APPFinanca.Views;

namespace APPFinanca
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginUsuarioPage), typeof(LoginUsuarioPage));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("UserId");

            var repos = Handler.MauiContext.Services.GetService<ITransactionRepository>();


            LoginUsuarioPage loginUsuarioPage = new LoginUsuarioPage(repos);
            // Redirecionar para a página de login
            Application.Current.MainPage = new NavigationPage(loginUsuarioPage);
        }
    }
}
