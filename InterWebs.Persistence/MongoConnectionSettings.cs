using System;
using MongoDB.Driver;

namespace InterWebs.Persistence
{
    public static class MongoConnectionSettings
    {
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

            return Environment.GetEnvironmentVariable("CUSTOMCONNSTR_MONGOLAB_URI")
                ?? Environment.GetEnvironmentVariable("CUSTOMCONNSTR_MONGOLAB_URI", EnvironmentVariableTarget.Machine);
        }
    }
}