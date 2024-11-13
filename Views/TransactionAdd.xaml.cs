using APPFinanca.Libraries.Utils.FixBugs;
using APPFinanca.Models;
using APPFinanca.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;
using static APPFinanca.Models.Enumation;

namespace APPFinanca.Views;

public partial class TransactionAdd : ContentPage
{
    private ITransactionRepository _repository;
    private string _userIdString = Preferences.Get("UserId", string.Empty);
    public TransactionAdd(ITransactionRepository repository)
    {
        InitializeComponent();
        _repository = repository;
        LoadTypes();
    }

    private void LoadTypes()
    {
        var paymentTypes = Enum.GetValues(typeof(PaymentType))
                            .Cast<PaymentType>()
                            .Where(e => e != PaymentType.Selecione)  // Exclui o placeholder
                            .Select(e => e.ToString())
                            .ToList();

        paymentTypes.Insert(0, "Selecione um tipo de pagamento");

        pickerTipoPagamento.ItemsSource = paymentTypes;

        pickerTipoPagamento.SelectedItem = paymentTypes.First();


        var groupTypes = Enum.GetValues(typeof(GroupType))
                           .Cast<GroupType>()
                           .Where(e => e != GroupType.Selecione)
                           .Select(e => e.ToString())
                           .ToList();

        groupTypes.Insert(0, "Selecione o Grupo");
        pickerTipoGrupo.ItemsSource = groupTypes;
        pickerTipoGrupo.SelectedItem = paymentTypes.First();
    }

    private void TapGestureRecognizerTapped_ToClose(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        KeyboardFixBugs.HideKeyboard();
        Navigation.PopModalAsync();
    }

    private void OnButtonClicked_Save(System.Object sender, System.EventArgs e)
    {
        if (IsValidData() == false)
            return;
        SaveTransactionInDatabase();

        KeyboardFixBugs.HideKeyboard();
        Navigation.PopModalAsync();

        WeakReferenceMessenger.Default.Send<string>(string.Empty);
    }

    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction()
        {
            TransactionType = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expenses,
            PaymentType = pickerTipoPagamento.SelectedIndex != -1 ?
                  (PaymentType)pickerTipoPagamento.SelectedIndex : PaymentType.Selecione,
            Group = pickerTipoGrupo.SelectedIndex != -1 ? (GroupType)pickerTipoGrupo.SelectedIndex : GroupType.Selecione,
            Name = EntryName.Text,
            Date = DatePickerDate.Date,
            Value = Math.Abs(decimal.Parse(EntryValue.Text.Replace("R$", "").Trim())),
            UserId = new Guid(_userIdString)
        };

        _repository.Add(transaction);
    }

    private bool IsValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        if (string.IsNullOrEmpty(EntryName.Text) || string.IsNullOrWhiteSpace(EntryName.Text))
        {
            sb.AppendLine("O campo 'Nome' deve ser preenchido!");
            valid = false;
        }
        if (string.IsNullOrEmpty(EntryValue.Text) || string.IsNullOrWhiteSpace(EntryValue.Text))
        {
            sb.AppendLine("O campo 'Valor' deve ser preenchido!");
            valid = false;
        }
        double result;
        if (!string.IsNullOrEmpty(EntryValue.Text) && !double.TryParse(EntryValue.Text.Replace("R$", "").Trim(), out result))
        {
            sb.AppendLine("O campo 'Valor' inválido!");
            valid = false;
        }


        if (valid == false)
        {
            LabelError.IsVisible = true;
            LabelError.Text = sb.ToString();
        }
        return valid;
    }
    private void OnEntryValueTextChanged(object sender, TextChangedEventArgs e)
    {
        if (EntryValue.Text != null)
        {
            // Remove qualquer caractere não numérico (exceto a vírgula) e o símbolo "R$"
            string cleanText = new string(EntryValue.Text.Where(c => Char.IsDigit(c) || c == ',').ToArray());

            // Se o texto não começar com "R$", adiciona o símbolo de moeda
            if (!cleanText.StartsWith("R$"))
            {
                cleanText = "R$ " + cleanText;
            }

            // Previne a entrada de mais de um separador decimal (vírgula)
            if (cleanText.Count(c => c == ',') > 1)
            {
                cleanText = cleanText.Remove(cleanText.LastIndexOf(','));
            }

            // Atualiza o texto no Entry
            EntryValue.Text = cleanText;

            // Define a posição do cursor para o final do texto
            EntryValue.CursorPosition = EntryValue.Text.Length;
        }
    }
}