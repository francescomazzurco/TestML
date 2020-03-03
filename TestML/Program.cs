using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestML
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            try
            {
                var mlContext = new MLContext();
                var models = ReadCsv(@"data\data.csv");
                var dataView = BuildDataView(mlContext, models);
                var experimentSettings = new RegressionExperimentSettings
                {
                    MaxExperimentTimeInSeconds = 600,
                    CacheDirectory = new DirectoryInfo(@".\cache"),
                };
                var experiment = mlContext.Auto().CreateRegressionExperiment(experimentSettings);
                // Data has already been parsed using invariant culture 
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                var bestRun = experiment.Execute(dataView).BestRun;
                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.ReadLine();
            }
        }


        private List<Model> ReadCsv(string path) 
        {
            // label is the first column
            return File.ReadAllLines(path)
                .Select(l => new Model(l.Split(',').Select(n => float.Parse(n, CultureInfo.InvariantCulture))))
                .ToList();
        }

        private IDataView BuildDataView(MLContext mlcontext, IEnumerable<Model> models)
        {
            // I'm not loading data the conventional way because in my production scenario
            // the number of features can be determined only at runtime by examining the database
            var schemaDefinition = SchemaDefinition.Create(typeof(Model));
            var vectorItemType = ((VectorDataViewType)schemaDefinition[0].ColumnType).ItemType;
            schemaDefinition[0].ColumnType = new VectorDataViewType(vectorItemType, models.First().Features.Length);
            return mlcontext.Data.LoadFromEnumerable(models, schemaDefinition);
        }
    }
}
