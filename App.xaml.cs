using APPFinanca.Views;

namespace APPFinanca
{
    public partial class App : Application
    {
        public App(LoginUsuarioPage loginUsuarioPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(loginUsuarioPage);

        }
    }
}
