namespace DbUp.MongoDB
{
    public class MongoDbSettings
    {
        public MongoDbSettings(string connectionString, string initialDatabase, string journalDatabase, string journalCollection)
        {
            ConnectionString = connectionString;
            InitialDatabase = initialDatabase;
            JournalDatabase = journalDatabase;
            JournalCollection = journalCollection;
        }

        public string ConnectionString { get; private set; }
        public string InitialDatabase { get; private set; }
        public string JournalDatabase { get; private set; }
        public string JournalCollection { get; private set; }
    }
}
