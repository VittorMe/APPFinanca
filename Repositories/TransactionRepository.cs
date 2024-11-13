using APPFinanca.Models;
using LiteDB;

namespace APPFinanca.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly LiteDatabase _database;
        private readonly string collectionName = "transactions";

        public TransactionRepository(LiteDatabase database)
        {
            _database = database;
        }


        public List<Transaction> GetTransactionsByUserId(Guid userId)
        {
            var transactionsCollection = _database.GetCollection<Transaction>(collectionName);

            // Buscar todas as transações onde o UserId corresponde ao passado
            return transactionsCollection.Find(t => t.UserId == userId).ToList();
        }


        public void Add(Transaction transaction)
        {
            var transactionsCollection = _database.GetCollection<Transaction>(collectionName);
            transactionsCollection.Insert(transaction);
        }

        public void Delete(Transaction transaction)
        {
            var transactionsCollection = _database.GetCollection<Transaction>(collectionName);
            transactionsCollection.Delete(transaction.Id);
        }


        public User GetUse(string Name, string Password)
        {
            var usersCollection = _database.GetCollection<User>("users");

            var user = usersCollection.FindOne(u => u.Name == Name && u.Password == Password);

            return user;
        }
        public User GetUseById(Guid userId)
        {
            var usersCollection = _database.GetCollection<User>("users");

            return usersCollection.Find(t => t.Id == userId).FirstOrDefault();
        }
        public void Update(Transaction transaction)
        {
            var transactionsCollection = _database.GetCollection<Transaction>(collectionName);
            transactionsCollection.Update(transaction);
        }

        public void AddUser(User user)
        {
            var usersCollection = _database.GetCollection<User>("users");

            var existingUser = usersCollection.FindOne(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase));

            if (existingUser != null)
            {
                throw new Exception("Usuário já cadastrado.");
            }

            // Inserir o novo usuário
            usersCollection.Insert(user);
        }
    }
}
