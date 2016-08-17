using System;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.MongoDB;
using DbUp.MongoDB.Model;
using MongoDB.Driver;
using static System.String;

namespace DbUp.MongoDb
{
    public class MongoDbJournal : IJournal
    {
        private Func<IUpgradeLog> log;
        private MongoDbSettings settings;

        /// <summary>
        /// Creates a new MongoDb collectionName journal.
        /// </summary>
        /// <param name="settings">The MongoDb connection settings.</param>
        /// <param name="logger">The upgrade logger.</param>
        public MongoDbJournal(MongoDbSettings settings, Func<IUpgradeLog> logger)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if(logger == null)
                throw new ArgumentNullException(nameof(logger));
            
            if(IsNullOrEmpty(settings.JournalDatabase))
                throw new ArgumentNullException(nameof(settings.JournalDatabase));

            if(IsNullOrEmpty(settings.JournalCollection))
                throw new ArgumentNullException(nameof(settings.JournalCollection));

            this.settings = settings;
            this.log = logger;
        }


        public string[] GetExecutedScripts()
        {
            var upgrades = GetJournalCollection().AsQueryable().ToList();

            return upgrades.Select(u => u.ScriptName).ToArray();
        }

        public void StoreExecutedScript(SqlScript script)
        {
            var upgradeCollection = GetJournalCollection();
            var maxSchemaVersionId = upgradeCollection
                .AsQueryable()
                .OrderByDescending(u => u.SchemaVersionId)
                .FirstOrDefault()?.SchemaVersionId ?? 0;
            
            var schemaVersion = new SchemaVersion()
            {
                SchemaVersionId = maxSchemaVersionId + 1,
                ScriptName = script.Name,
                Applied = DateTime.Now
            };

            upgradeCollection.InsertOne(schemaVersion);
        }

        private IMongoCollection<SchemaVersion> GetJournalCollection()
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.JournalDatabase);
            var collection = database.GetCollection<SchemaVersion>(settings.JournalCollection);

            return collection;
        }
    }
}
