using DbUp.Builder;
using DbUp.MongoDB;
using DbUp.MongoDB.ScriptProviders;
using DbUp.ScriptProviders;

namespace DbUp.MongoDb
{
    public static class MongoDbExtentions
    {
        /// <summary>
        /// Creates an upgrader for MongoDb databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">MongoDB database connection string.</param>
        /// <returns>
        /// A builder for a database upgrader designed for MongoDB databases.
        /// </returns>
        public static UpgradeEngineBuilder MongoDbDatabase(this SupportedDatabases supported, string connectionString)
        {
            var mongoDbSettings = new MongoDbSettings(connectionString, "Temp", "SchemaVersion", "SchemaVersion");

            var builder = new UpgradeEngineBuilder();
            builder.Configure(c => c.ConnectionManager = new UnsupportedConnectionManager());
            builder.Configure(c => c.ScriptExecutor = new MongoDbScriptExecutor(mongoDbSettings, () => c.Log, () => c.VariablesEnabled, c.ScriptPreprocessors));
            builder.Configure(c => c.Journal = new MongoDbJournal(mongoDbSettings, () => c.Log));
            return builder;
        }

        /// <summary>
        /// Tracks the list of executed scripts in a MongoDb collection.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="connectionString">The connection string for the MongoDb server.</param>
        /// <param name="database">The database for the journal.</param>
        /// <param name="collection">The collection for the journal.</param>
        /// <returns></returns>
        public static UpgradeEngineBuilder JournalToMongoDb(this UpgradeEngineBuilder builder, string connectionString, string database, string collection)
        {
            var mongoDbSettings = new MongoDbSettings(connectionString, null, database, collection);

            builder.Configure(c => c.Journal = new MongoDbJournal(mongoDbSettings, () => c.Log));
            return builder;
        }


        /// <summary>
        /// Adds all scripts from a folder on the file system, with custom options (Encoding, filter, etc.).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="path">The directory path.</param>
        /// <param name="options">Options for the file System Provider</param>
        /// <returns>
        /// The same builder
        /// </returns>
        public static UpgradeEngineBuilder WithScriptsFromFileSystemEx(this UpgradeEngineBuilder builder, string path, FileSystemScriptOptions options)
        {

            builder.Configure(c => c.ScriptProviders.Add(new FileSystemScriptProviderEx(path, options)));
            return builder;
        }
    }
}
