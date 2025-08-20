using APPFinanca.Models;
using APPFinanca.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using static APPFinanca.Models.Enumation;

namespace APPFinanca.Views;

public partial class TransactionList : ContentPage
{
    private ITransactionRepository _repository;
    private string _userIdString = Preferences.Get("UserId", string.Empty);
    private List<Transaction> _allTransactions = new List<Transaction>();

    public TransactionList(ITransactionRepository repository)
    {
        this._repository = repository;

        NavigationPage.SetHasBackButton(this, false);
        InitializeComponent();

        Reload();

        WeakReferenceMessenger.Default.Register<string>(this, (e, msg) =>
        {
            Reload();
        });
    }

    private void Reload()
    {
        _allTransactions = _repository.GetTransactionsByUserId(new Guid(_userIdString));
        
        // Ordena as transações por data (mais recentes primeiro)
        var sortedTransactions = _allTransactions
            .OrderByDescending(t => t.Date)
            .ToList();

        CollectionViewTransactions.ItemsSource = sortedTransactions;
        
        UpdateBalanceInfo();
    }

    private void UpdateBalanceInfo()
    {
        decimal income = _allTransactions
            .Where(a => a.TransactionType == TransactionType.Income)
            .Sum(a => a.Value);
            
        decimal expense = _allTransactions
            .Where(a => a.TransactionType == TransactionType.Expenses)
            .Sum(a => a.Value);
            
        decimal balance = income - expense;

        LabelIncome.Text = income.ToString("C");
        LabelExpense.Text = expense.ToString("C");
        LabelBalance.Text = balance.ToString("C");
        
        // Muda a cor do saldo baseado se é positivo ou negativo
        if (balance >= 0)
        {
            LabelBalance.TextColor = Colors.Green;
        }
        else
        {
            LabelBalance.TextColor = Colors.Red;
        }
    }

    private void OnButtonClicked_To_TransactionAdd(Object sender, EventArgs e)
    {
        var transactionAdd = Handler.MauiContext.Services.GetService<TransactionAdd>();
        Navigation.PushModalAsync(transactionAdd);
    }

    private void TapGestureRecognizerTapped_To_TransactionEdit(object sender, TappedEventArgs e)
    {
        var grid = (Grid)sender;
        var gesture = (TapGestureRecognizer)grid.GestureRecognizers[0];
        Transaction transaction = (Transaction)gesture.CommandParameter;

        var transactionEdit = Handler.MauiContext.Services.GetService<TransactionEdit>();
        transactionEdit.SetTransactionToEdit(transaction);
        Navigation.PushModalAsync(transactionEdit);
    }

    private async void TapGestureRecognizerTapped_ToDelete(object sender, TappedEventArgs e)
    {
        await AnimationBorder((Border)sender, true);
        
        bool result = await App.Current.MainPage.DisplayAlert(
            "Excluir Transação", 
            "Tem certeza que deseja excluir esta transação?", 
            "Sim", 
            "Não");

        if (result)
        {
            Transaction transaction = (Transaction)e.Parameter;
            _repository.Delete(transaction);

            Reload();
            WeakReferenceMessenger.Default.Send<string>(string.Empty);
        }
        else
        {
            await AnimationBorder((Border)sender, false);
        }
    }

    private Color _borderOriginalBackgroundColor;
    private String _labelOriginalText;
    
    private async Task AnimationBorder(Border border, bool IsDeleteAnimation)
    {
        var label = (Label)border.Content;

        if (IsDeleteAnimation)
        {
            _borderOriginalBackgroundColor = border.BackgroundColor;
            _labelOriginalText = label.Text;

            await border.RotateYTo(90, 500);

            border.BackgroundColor = Colors.Red;
            label.TextColor = Colors.White;
            label.Text = "X";

            await border.RotateYTo(180, 500);
        }
        else
        {
            await border.RotateYTo(90, 500);

            border.BackgroundColor = _borderOriginalBackgroundColor;
            label.TextColor = Colors.Black;
            label.Text = _labelOriginalText;

            await border.RotateYTo(0, 500);
        }
    }

    // Método para filtrar transações por tipo
    private void FilterByType(TransactionType? type = null)
    {
        var filteredTransactions = type.HasValue 
            ? _allTransactions.Where(t => t.TransactionType == type.Value).ToList()
            : _allTransactions;

        var sortedTransactions = filteredTransactions
            .OrderByDescending(t => t.Date)
            .ToList();

        CollectionViewTransactions.ItemsSource = sortedTransactions;
    }

    // Método para buscar transações por texto
    private void SearchTransactions(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            Reload();
            return;
        }

        var searchResults = _allTransactions
            .Where(t => 
                t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (t.Description != null && t.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                (t.Location != null && t.Location.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(t => t.Date)
            .ToList();

        CollectionViewTransactions.ItemsSource = searchResults;
    }

    // Eventos dos filtros
    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchTransactions(e.NewTextValue);
    }

    private void OnFilterAllClicked(object sender, EventArgs e)
    {
        Reload();
        UpdateFilterButtonStates(sender as Button);
    }

    private void OnFilterIncomeClicked(object sender, EventArgs e)
    {
        FilterByType(TransactionType.Income);
        UpdateFilterButtonStates(sender as Button);
    }

    private void OnFilterExpenseClicked(object sender, EventArgs e)
    {
        FilterByType(TransactionType.Expenses);
        UpdateFilterButtonStates(sender as Button);
    }

    private void UpdateFilterButtonStates(Button activeButton)
    {
        // Reset all buttons
        var filterButtons = new[] { 
            (Button)FindByName("FilterAllButton"),
            (Button)FindByName("FilterIncomeButton"), 
            (Button)FindByName("FilterExpenseButton") 
        };

        foreach (var button in filterButtons)
        {
            if (button != null)
            {
                button.BackgroundColor = Color.FromArgb("#E9ECEF");
                button.TextColor = Colors.Black;
            }
        }

        // Highlight active button
        if (activeButton != null)
        {
            activeButton.BackgroundColor = Color.FromArgb("#007ACC");
            activeButton.TextColor = Colors.White;
        }
    }
}