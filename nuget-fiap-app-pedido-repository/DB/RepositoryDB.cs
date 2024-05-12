using MongoDB.Driver;
using System;

namespace nuget_fiap_app_pedido_repository.DB
{
    public sealed class RepositoryDB
    {
        private Guid _id;
        public IMongoDatabase Database { get; private set; }

        public RepositoryDB()
        {
            _id = Guid.NewGuid();
            var client = GetMongoClient();
            Database = client.GetDatabase("DB"); 
        }


        /// <summary>
        /// Cria e retorna um MongoClient com base nas variáveis de ambiente
        /// </summary>
        /// <returns>MongoClient</returns>
        private static MongoClient GetMongoClient()
        {
            CreateEnvironmentVariables();

            var senhaBase = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
            var userBase = Environment.GetEnvironmentVariable("DB_USER") ?? "";
            var hostBase = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"; // Use o hostname padrão ou o fornecido

            var connectionString = $"mongodb://{userBase}:{senhaBase}@{hostBase}";
            return new MongoClient(connectionString);
        }

        /// <summary>
        /// Configura as variáveis de ambiente necessárias
        /// </summary>
        private static void CreateEnvironmentVariables()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_PASSWORD")))
                Environment.SetEnvironmentVariable("DB_PASSWORD", "pass123"); // Senha padrão

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_USER")))
                Environment.SetEnvironmentVariable("DB_USER", "admin"); // Usuário padrão

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_HOST")))
                Environment.SetEnvironmentVariable("DB_HOST", "localhost:27017"); // Host padrão
        }
    }
}
