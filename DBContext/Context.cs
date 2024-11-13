using APPFinanca.Repositories;
using LiteDB;

namespace APPFinanca.DBContext
{
    public static class Context
    {
        private static string DatabaseName = "database.db";
        private static string DatabaseDirectory = FileSystem.AppDataDirectory;
        public static string DatabasePath = Path.Combine(DatabaseDirectory, DatabaseName);


        public static MauiAppBuilder RegisterDatabaseAndRepositories(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<LiteDatabase>(
                options =>
                {
                    return new LiteDatabase($"Filename={DatabasePath};Connection=Shared");
                }
            );
            mauiAppBuilder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
            return mauiAppBuilder;
        }
    }
}
