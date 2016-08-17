using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;
using DbUp.MongoDB;
using DbUp.MongoDB.Utils;
using MongoDB.Driver;

namespace DbUp.MongoDb
{
    public class MongoDbScriptExecutor : IScriptExecutor
    {
        private Func<IUpgradeLog> log;
        private Func<bool> variablesEnabled;
        private IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private MongoDbSettings settings;

        public MongoDbScriptExecutor(MongoDbSettings settings,
                                     Func<IUpgradeLog> log,
                                     Func<bool> variablesEnabled,
                                     IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
            this.settings = settings;
        }

        public void Execute(SqlScript script)
        {
            Execute(script, null);
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="variables">Variables to replace in the script</param>
        public void Execute(SqlScript script, IDictionary<string, string> variables)
        {
            if (variables == null)
                variables = new Dictionary<string, string>();

            log().WriteInformation("Executing MongoDb js script '{0}'", script.Name);

            var contents = script.Contents;

            if (variablesEnabled())
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);
            contents = (scriptPreprocessors??new IScriptPreprocessor[0])
                .Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));

            try
            {
                var defaultDatabase = GetDefaultDatabase();
                var result = defaultDatabase.Eval(contents);

                if(result != null)
                    log().WriteInformation("Script has been executed with result: '{0}'", result);
            }
            catch (Exception ex)
            {
                log().WriteInformation("Exception has occured in script: '{0}'", script.Name);
                log().WriteError(ex.ToString());
                throw;
            }
        }

        public void VerifySchema()
        {
            // MongoDb doesn't support schema;
        }

        private IMongoDatabase GetDefaultDatabase()
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.InitialDatabase);

            return mongoDatabase;
        }

        public int? ExecutionTimeoutSeconds { get; set; }
    }
}