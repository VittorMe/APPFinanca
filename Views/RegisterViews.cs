namespace APPFinanca.Views
{
    static class RegisterViews
    {

        public static MauiAppBuilder Register(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<LoginUsuarioPage>();
            mauiAppBuilder.Services.AddTransient<TransactionList>();
            mauiAppBuilder.Services.AddTransient<TransactionAdd>();
            mauiAppBuilder.Services.AddTransient<TransactionEdit>();
            mauiAppBuilder.Services.AddTransient<Dashboard>();
            return mauiAppBuilder;
        }
    }
}
