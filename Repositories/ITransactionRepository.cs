using APPFinanca.Models;

namespace APPFinanca.Repositories
{
    public interface ITransactionRepository
    {

        void Add(Transaction transaction);
        void Delete(Transaction transaction);
        List<Transaction> GetTransactionsByUserId(Guid userId);
        void Update(Transaction transaction);

        //Login
        User GetUse(string Name, string Password);
        User GetUseById(Guid userId);
        void AddUser(User user);
    }
}
