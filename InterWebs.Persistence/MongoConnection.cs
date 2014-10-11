using System.Text;
using MongoDB.Driver;

namespace InterWebs.Persistence
{
    public static class MongoConnection
    {
        private static MongoClient _mongoClient;

        private static MongoClient MongoClient
        {
            get { return _mongoClient ?? (_mongoClient = new MongoClient(GetConnectionString())); }
        }

        private static MongoServer MongoServer
        {
            get { return MongoClient.GetServer(); }
        }

        internal static MongoDatabase GetDatabase()
        {
            return MongoServer.GetDatabase("InterWebs");
        }

        private static string GetConnectionString()
        {
            var connectionStringBuilder = new StringBuilder();
            var serverName = GetServerName();

            if (!serverName.StartsWith("mongodb://"))
                connectionStringBuilder.Append("mongodb://");

            connectionStringBuilder.Append(serverName);

            return connectionStringBuilder.ToString();
        }

        private static string GetServerName()
        {
            return "localhost";
        }
    }
}