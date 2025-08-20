using APPFinanca.Models;
using APPFinanca.Repositories;

namespace APPFinanca.Views;

public partial class LoginUsuarioPage : ContentPage
{
    private ITransactionRepository _repository;

    public LoginUsuarioPage(ITransactionRepository repository)
    {
        this._repository = repository;
        InitializeComponent();

    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        string usuario = txtUsuario.Text?.Trim();
        string senha = txtSenha.Text?.Trim();

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
        {
            DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
            return;
        }
        var user = _repository.GetUse(usuario, senha);

        if (user == null)
        {
            // Exibe um alerta perguntando se o usu�rio deseja se cadastrar
            var cadastrar = await DisplayAlert("Usu�rio n�o encontrado", "Deseja cadastrar este usu�rio?", "Sim", "N�o");

            if (cadastrar)
            {


                var novoUsuario = new User
                {
                    Name = usuario,
                    Password = senha // Considere aplicar hashing na senha
                };


                Preferences.Set("UserId", novoUsuario.Id.ToString());
                try
                {
                    _repository.AddUser(novoUsuario);
                    await DisplayAlert("Sucesso", "Usu�rio cadastrado com sucesso!", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Cancelado", "Cadastro de usu�rio foi cancelado.", "OK");
            }
        }
        else
        {
            Preferences.Set("UserId", user.Id.ToString());

            // await Navigation.PushAsync(new TransactionList());
            Application.Current.MainPage = new AppShell();
        }
    }
}