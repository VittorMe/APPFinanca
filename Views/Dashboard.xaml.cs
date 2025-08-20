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
    private List<Transaction> _transactions = new List<Transaction>();
    private List<Transaction> _filteredTransactions = new List<Transaction>();

    public Dashboard(ITransactionRepository repository)
    {
        InitializeComponent();
        _repository = repository;

        InitializePickers();
        Reload();

        WeakReferenceMessenger.Default.Register<string>(this, (e, msg) =>
        {
            Reload();
        });
    }

    private void InitializePickers()
    {
        // Preencher o MonthPicker com os meses
        MonthPicker.ItemsSource = Enum.GetValues(typeof(Month)).Cast<Month>().ToList();
        MonthPicker.SelectedIndex = DateTime.Now.Month - 1; // Mês atual

        // Preencher o YearPicker com os últimos 5 anos
        var currentYear = DateTime.Now.Year;
        var years = Enumerable.Range(currentYear - 4, 5).ToList();
        YearPicker.ItemsSource = years;
        YearPicker.SelectedItem = currentYear;
    }

    private void Reload()
    {
        _user = _repository.GetUseById(new Guid(_userIdString));
        _transactions.Clear();
        _transactions.AddRange(_repository.GetTransactionsByUserId(new Guid(_userIdString)));

        UpdateDashboard();
    }

    private void UpdateDashboard()
    {
        // Filtrar transações pelo período selecionado
        FilterTransactionsByPeriod();

        // Atualizar informações do usuário
        UserNameLabel.Text = $"Olá, {_user.Name}!";

        // Calcular e atualizar resumo financeiro
        UpdateFinancialSummary();

        // Atualizar gráfico de distribuição por categoria
        UpdateCategoryDistribution();

        // Atualizar gráfico de evolução mensal
        UpdateMonthlyEvolution();

        // Atualizar análise detalhada
        UpdateDetailedAnalysis();

        // Atualizar top 5 transações
        UpdateTopTransactions();
    }

    private void FilterTransactionsByPeriod()
    {
        var selectedMonth = MonthPicker.SelectedIndex >= 0 ? MonthPicker.SelectedIndex + 1 : DateTime.Now.Month;
        var yearPicker = YearPicker.SelectedItem as int? ?? DateTime.Now.Year;
        var selectedYear = (int)yearPicker;

        _filteredTransactions = _transactions
            .Where(t => t.Date.Month == selectedMonth && t.Date.Year == selectedYear)
            .ToList();
    }

    private void UpdateFinancialSummary()
    {
        var totalSpent = _filteredTransactions
            .Where(t => t.TransactionType == TransactionType.Expenses)
            .Sum(t => t.Value);

        var totalReceived = _filteredTransactions
            .Where(t => t.TransactionType == TransactionType.Income)
            .Sum(t => t.Value);

        var balance = totalReceived - totalSpent;

        // Atualizar labels principais
        BalanceLabel.Text = balance.ToString("C");
        BalanceLabel.TextColor = balance >= 0 ? Colors.Green : Colors.Red;

        TotalReceivedLabel.Text = totalReceived.ToString("C");
        TotalSpentLabel.Text = totalSpent.ToString("C");

        // Calcular média diária
        var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        var dailyAverage = (totalSpent + totalReceived) / daysInMonth;
        DailyAverageLabel.Text = dailyAverage.ToString("C");

        // Maior transação
        var largestTransaction = _filteredTransactions
            .OrderByDescending(t => t.Value)
            .FirstOrDefault();

        if (largestTransaction != null)
        {
            LargestTransactionLabel.Text = largestTransaction.Value.ToString("C");
        }
        else
        {
            LargestTransactionLabel.Text = "R$ 0,00";
        }

        // Total de transações
        TotalTransactionsLabel.Text = _filteredTransactions.Count.ToString();
    }

    private void UpdateCategoryDistribution()
    {
        var categoryGroups = _filteredTransactions
            .GroupBy(t => t.Group)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Value) })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();

        var labels = new[] { Category1Label, Category2Label, Category3Label, Category4Label, Category5Label };
        var values = new[] { Category1Value, Category2Value, Category3Value, Category4Value, Category5Value };

        for (int i = 0; i < 5; i++)
        {
            if (i < categoryGroups.Count)
            {
                labels[i].Text = categoryGroups[i].Category.ToString();
                values[i].Text = categoryGroups[i].Total.ToString("C");
                labels[i].IsVisible = true;
                values[i].IsVisible = true;
            }
            else
            {
                labels[i].IsVisible = false;
                values[i].IsVisible = false;
            }
        }
    }

    private void UpdateMonthlyEvolution()
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var monthLabels = new[] { Month1Label, Month2Label, Month3Label, Month4Label, Month5Label, Month6Label };
        var incomeBars = new[] { IncomeBar1, IncomeBar2, IncomeBar3, IncomeBar4, IncomeBar5, IncomeBar6 };
        var expenseBars = new[] { ExpenseBar1, ExpenseBar2, ExpenseBar3, ExpenseBar4, ExpenseBar5, ExpenseBar6 };
        var valueLabels = new[] { Value1Label, Value2Label, Value3Label, Value4Label, Value5Label, Value6Label };

        var maxValue = _transactions.Any() ? _transactions.Max(t => t.Value) : 1.0m;

        for (int i = 0; i < 6; i++)
        {
            var targetMonth = currentMonth - 5 + i;
            var targetYear = currentYear;

            if (targetMonth <= 0)
            {
                targetMonth += 12;
                targetYear--;
            }

            // Nome do mês
            monthLabels[i].Text = GetMonthName(targetMonth);

            // Dados do mês
            var monthTransactions = _transactions
                .Where(t => t.Date.Month == targetMonth && t.Date.Year == targetYear)
                .ToList();

            var monthIncome = monthTransactions
                .Where(t => t.TransactionType == TransactionType.Income)
                .Sum(t => t.Value);

            var monthExpense = monthTransactions
                .Where(t => t.TransactionType == TransactionType.Expenses)
                .Sum(t => t.Value);

            // Altura das barras (proporcional ao valor máximo)
            var incomeHeight = maxValue > 0 ? (monthIncome / maxValue) * 100 : 0;
            var expenseHeight = maxValue > 0 ? (monthExpense / maxValue) * 100 : 0;

            incomeBars[i].HeightRequest = (double)Math.Max(10, incomeHeight);
            expenseBars[i].HeightRequest = (double)Math.Max(10, expenseHeight);

            // Valor total do mês
            var monthTotal = monthIncome + monthExpense;
            valueLabels[i].Text = monthTotal > 0 ? monthTotal.ToString("C") : "";
        }
    }

    private string GetMonthName(int month)
    {
        return month switch
        {
            1 => "Jan",
            2 => "Fev",
            3 => "Mar",
            4 => "Abr",
            5 => "Mai",
            6 => "Jun",
            7 => "Jul",
            8 => "Ago",
            9 => "Set",
            10 => "Out",
            11 => "Nov",
            12 => "Dez",
            _ => ""
        };
    }

    private void UpdateDetailedAnalysis()
    {
        if (!_filteredTransactions.Any())
        {
            HighestSpendingDayLabel.Text = "N/A";
            MostExpensiveCategoryLabel.Text = "N/A";
            MostUsedPaymentLabel.Text = "N/A";
            RecurringTransactionsLabel.Text = "0";
            return;
        }

        // Dia com mais gastos
        var highestSpendingDay = _filteredTransactions
            .Where(t => t.TransactionType == TransactionType.Expenses)
            .GroupBy(t => t.Date.Day)
            .Select(g => new { Day = g.Key, Total = g.Sum(t => t.Value) })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        if (highestSpendingDay != null)
        {
            HighestSpendingDayLabel.Text = $"{highestSpendingDay.Day}º dia";
        }
        else
        {
            HighestSpendingDayLabel.Text = "N/A";
        }

        // Categoria mais cara
        var mostExpensiveCategory = _filteredTransactions
            .GroupBy(t => t.Group)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Value) })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        if (mostExpensiveCategory != null)
        {
            MostExpensiveCategoryLabel.Text = mostExpensiveCategory.Category.ToString();
        }
        else
        {
            MostExpensiveCategoryLabel.Text = "N/A";
        }

        // Forma de pagamento mais usada
        var mostUsedPayment = _filteredTransactions
            .GroupBy(t => t.PaymentType)
            .Select(g => new { Payment = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        if (mostUsedPayment != null)
        {
            MostUsedPaymentLabel.Text = mostUsedPayment.Payment.ToString();
        }
        else
        {
            MostUsedPaymentLabel.Text = "N/A";
        }

        // Transações recorrentes
        var recurringCount = _filteredTransactions.Count(t => t.IsRecurring);
        RecurringTransactionsLabel.Text = recurringCount.ToString();
    }

    private void UpdateTopTransactions()
    {
        var topTransactions = _filteredTransactions
            .OrderByDescending(t => t.Value)
            .Take(5)
            .ToList();

        TopTransactionsCollectionView.ItemsSource = topTransactions;
    }

    private void OnMonthSelected(object sender, EventArgs e)
    {
        UpdateDashboard();
    }

    private void OnYearSelected(object sender, EventArgs e)
    {
        UpdateDashboard();
    }
}