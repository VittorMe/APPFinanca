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
        SetupInitialState();
    }

    private void SetupInitialState()
    {
        // Define a data atual como padrão
        DatePickerDate.Date = DateTime.Today;
        
        // Configura o switch de recorrência
        SwitchRecurring.IsToggled = false;
        PickerRecurrenceType.IsVisible = false;
    }

    private void LoadTypes()
    {
        // Carrega tipos de pagamento
        var paymentTypes = Enum.GetValues(typeof(PaymentType))
                            .Cast<PaymentType>()
                            .Where(e => e != PaymentType.Selecione)
                            .Select(e => e.ToString())
                            .ToList();

        paymentTypes.Insert(0, "Selecione um tipo de pagamento");
        pickerTipoPagamento.ItemsSource = paymentTypes;
        pickerTipoPagamento.SelectedItem = paymentTypes.First();

        // Carrega tipos de grupo/categoria
        var groupTypes = Enum.GetValues(typeof(GroupType))
                           .Cast<GroupType>()
                           .Where(e => e != GroupType.Selecione)
                           .Select(e => e.ToString())
                           .ToList();

        groupTypes.Insert(0, "Selecione a Categoria");
        pickerTipoGrupo.ItemsSource = groupTypes;
        pickerTipoGrupo.SelectedItem = groupTypes.First();

        // Carrega tipos de recorrência
        var recurrenceTypes = Enum.GetValues(typeof(RecurrenceType))
                               .Cast<RecurrenceType>()
                               .Where(e => e != RecurrenceType.Nenhuma)
                               .Select(e => e.ToString())
                               .ToList();

        recurrenceTypes.Insert(0, "Selecione o tipo de recorrência");
        PickerRecurrenceType.ItemsSource = recurrenceTypes;
        PickerRecurrenceType.SelectedItem = recurrenceTypes.First();
    }

    private void TapGestureRecognizerTapped_ToClose(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        KeyboardFixBugs.HideKeyboard();
        Navigation.PopModalAsync();
    }

    private void OnSwitchRecurringToggled(object sender, ToggledEventArgs e)
    {
        PickerRecurrenceType.IsVisible = e.Value;
        
        if (!e.Value)
        {
            PickerRecurrenceType.SelectedItem = PickerRecurrenceType.ItemsSource.Cast<string>().First();
        }
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
            Name = EntryName.Text?.Trim(),
            Description = string.IsNullOrWhiteSpace(EditorDescription.Text) ? null : EditorDescription.Text?.Trim(),
            Location = string.IsNullOrWhiteSpace(EntryLocation.Text) ? null : EntryLocation.Text?.Trim(),
            Date = DatePickerDate.Date,
            Value = Math.Abs(decimal.Parse(EntryValue.Text.Replace("R$", "").Trim())),
            IsRecurring = SwitchRecurring.IsToggled,
            RecurrenceType = SwitchRecurring.IsToggled && PickerRecurrenceType.SelectedIndex > 0 ?
                (RecurrenceType)PickerRecurrenceType.SelectedIndex : null,
            UserId = new Guid(_userIdString)
        };

        _repository.Add(transaction);
    }

    private bool IsValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        // Validação do nome
        if (string.IsNullOrEmpty(EntryName.Text) || string.IsNullOrWhiteSpace(EntryName.Text))
        {
            sb.AppendLine("• O campo 'Nome da transação' deve ser preenchido!");
            valid = false;
        }

        // Validação do valor
        if (string.IsNullOrEmpty(EntryValue.Text) || string.IsNullOrWhiteSpace(EntryValue.Text))
        {
            sb.AppendLine("• O campo 'Valor' deve ser preenchido!");
            valid = false;
        }
        else
        {
            double result;
            if (!double.TryParse(EntryValue.Text.Replace("R$", "").Trim(), out result))
            {
                sb.AppendLine("• O campo 'Valor' é inválido!");
                valid = false;
            }
            else if (result <= 0)
            {
                sb.AppendLine("• O valor deve ser maior que zero!");
                valid = false;
            }
        }

        // Validação do tipo de pagamento
        if (pickerTipoPagamento.SelectedIndex == 0)
        {
            sb.AppendLine("• Selecione um tipo de pagamento!");
            valid = false;
        }

        // Validação da categoria
        if (pickerTipoGrupo.SelectedIndex == 0)
        {
            sb.AppendLine("• Selecione uma categoria!");
            valid = false;
        }

        // Validação da recorrência
        if (SwitchRecurring.IsToggled && PickerRecurrenceType.SelectedIndex == 0)
        {
            sb.AppendLine("• Selecione o tipo de recorrência!");
            valid = false;
        }

        if (valid == false)
        {
            LabelError.IsVisible = true;
            LabelError.Text = sb.ToString();
        }
        else
        {
            LabelError.IsVisible = false;
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