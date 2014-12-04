using System;
using System.Text;
using MongoDB.Driver;

namespace InterWebs.Persistence
{
    public static class MongoConnection
    {
        private const bool Production = true;
        private static MongoClient mongoClient;

        private static MongoClient MongoClient
        {
            get { return mongoClient ?? (mongoClient = new MongoClient(GetConnectionString())); }
        }

        private static MongoServer MongoServer
        {
            get { return MongoClient.GetServer(); }
        }

        internal static MongoDatabase GetDatabase()
        {
            return MongoServer.GetDatabase("MongoLab-rs");
        }

        public static string GetConnectionString()
        {
            if (Production)
            {
                return Environment.GetEnvironmentVariable("CUSTOMCONNSTR_MONGOLAB_URI");
            }

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