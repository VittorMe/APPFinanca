using APPFinanca.Models;
using APPFinanca.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using static APPFinanca.Models.Enumation;

namespace APPFinanca.Views;

public partial class Dashboard : ContentPage
{
    private ITransactionRepository _repository;
    private string _userIdString = Preferences.Get("UserId", string.Empty);

    private User _user = new User();
    private List<APPFinanca.Models.Transaction> _transaction = new List<APPFinanca.Models.Transaction>();



    public Dashboard(ITransactionRepository repository)
    {
        InitializeComponent();
        _repository = repository;

        Reload();

        WeakReferenceMessenger.Default.Register<string>(this, (e, msg) =>
        {
            Reload();
        });
    }

    private void Reload()
    {
        _user = _repository.GetUseById(new Guid(_userIdString));
        _transaction.Clear();
        _transaction.AddRange(_repository.GetTransactionsByUserId(new Guid(_userIdString)));

        UserNameLabel.Text = _user.Name;

        // Calcular o total gasto e recebido
        var totalSpent = _transaction
            .Where(t => t.TransactionType == TransactionType.Expenses)
            .Sum(t => t.Value);

        var totalReceived = _transaction
            .Where(t => t.TransactionType == TransactionType.Income)
            .Sum(t => t.Value);

        // Atualizar os labels
        TotalSpentLabel.Text = $"R$ {totalSpent:N2}";
        TotalReceivedLabel.Text = $"R$ {totalReceived:N2}";

        // Agrupar as transações por 'Group' e somar os valores
        var groupedTransactions = _transaction
            .GroupBy(t => t.Group)  // Agrupar por 'Group' (você pode ajustar o nome conforme o seu modelo)
            .Select(g => new
            {
                Group = g.Key,
                TotalValue = g.Sum(t => t.Value) // Somar os valores do grupo
            })
            .ToList();

        // Vincular a coleção agrupada ao CollectionView
        TransactionsCollectionView.ItemsSource = groupedTransactions;

        // Preencher o MonthPicker com os meses do enum
        MonthPicker.ItemsSource = Enum.GetValues(typeof(Month)).Cast<Month>().ToList();
    }


    private void OnMonthSelected(object sender, EventArgs e)
    {
        // Obter o mês selecionado (começa em 1, pois SelectedIndex começa de 0)
        var selectedMonth = MonthPicker.SelectedIndex + 1;

        // Filtrar as transações do mês selecionado
        var filteredTransactions = _transaction
            .Where(t => t.Date.Month == selectedMonth)  // Filtro por mês
            .ToList();

        // Calcular o total gasto e recebido para o mês selecionado
        var totalSpent = filteredTransactions
            .Where(t => t.TransactionType == TransactionType.Expenses)
            .Sum(t => t.Value);

        var totalReceived = filteredTransactions
            .Where(t => t.TransactionType == TransactionType.Income)
            .Sum(t => t.Value);

        // Atualizar os labels
        TotalSpentLabel.Text = $"R$ {totalSpent:N2}";
        TotalReceivedLabel.Text = $"R$ {totalReceived:N2}";

        // Agrupar as transações filtradas por 'Group' e somar os valores
        var groupedTransactions = filteredTransactions
            .GroupBy(t => t.Group)  // Agrupar por 'Group' (você pode ajustar o nome conforme o seu modelo)
            .Select(g => new
            {
                Group = g.Key,
                TotalValue = g.Sum(t => t.Value) // Somar os valores do grupo
            })
            .ToList();

        // Vincular a coleção agrupada ao CollectionView
        TransactionsCollectionView.ItemsSource = groupedTransactions;
    }

}