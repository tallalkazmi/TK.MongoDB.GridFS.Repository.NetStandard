﻿using MongoDB.Driver;
using System;

namespace TK.MongoDB.GridFS
{
    internal class MongoDbContext : IDisposable
    {
        readonly string DatabaseName;
        MongoClient Client;

        /// <summary>
        /// Creates an instance of IMongoDatabase from connection string provided
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public MongoDbContext(string connectionString)
        {
            DatabaseName = new MongoUrl(connectionString).DatabaseName;
            Client = new MongoClient(connectionString);
        }

        /// <summary>
        /// Represents a database of type IMongoDatabase in MongoDB
        /// </summary>
        public IMongoDatabase Database
        {
            get { return Client.GetDatabase(DatabaseName); }
        }

        #region Dispose
        protected bool Disposed { get; private set; }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    if (Client != null)
                        Client = null;
                }
                this.Disposed = true;
            }
        }
        #endregion
    }
}
