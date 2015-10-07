﻿using System;
using System.Configuration;
using System.IO;
using Models.DocumentTypes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace QueryManager
{
    public class RavenDataStorage : IDataStorage
    {
        private string _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            @"Posizioni\Archive");

        private EmbeddableDocumentStore _documentStore;

        /// <summary>
        /// Initializes the database and creates the necessary indexes
        /// </summary>
        public void Initialize()
        {
            _documentStore = new EmbeddableDocumentStore { DataDirectory = ConnectionString};

            var turnOnManagementStudio = ConfigurationManager.AppSettings["ActivateManagementStudio"];
            if ("True" == turnOnManagementStudio)
            {
                _documentStore.UseEmbeddedHttpServer = true;
                _documentStore.Configuration.ServerName = "localhost";
                _documentStore.Configuration.Port = 6666;
            }

            var alwaysWaitForLastWrite = ConfigurationManager.AppSettings["AlwaysWaitForLastWrite"];
            if ("True" == alwaysWaitForLastWrite)
            {
                _documentStore.Conventions.DefaultQueryingConsistency =
                    ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            }

            _documentStore.Conventions.RegisterIdConvention<SaleConfirmation>((dbname, commands, entity) => _documentStore.Conventions.GetTypeTagName(entity.GetType()) + "/");
            _documentStore.Initialize();
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            IndexCreation.CreateIndexes(GetType().Assembly, _documentStore);
        }

        public string ConnectionString
        {
            get
            {
                return _dataDirectory;
            }
            set
            {
                _dataDirectory = value;
            }
        }

        public EmbeddableDocumentStore DocumentStore
        {
            get { return _documentStore; }
        }

        public IDocumentSession CreateSession()
        {
            return _documentStore.OpenSession();
        }
    }
}
