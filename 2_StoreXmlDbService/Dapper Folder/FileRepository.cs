using Confluent.Kafka;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StoreXmlDbService;
using StoreXmlDbService.Model;
using StoreXmlDbService.DapperFolder;

namespace StoreXmlDbService.DapperFolder
{
    public class FileRepository : IFileRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ConsumerConfig _config;

        public FileRepository(DapperContext dapperContext, ConsumerConfig config)
        {
            _dapperContext = dapperContext;
            _config = config;
        }
        //private static readonly string fileLocation = @"C:\Users\Karthik.c\source\repos\Dotnet-Poc's\StoreXmlInDbViaAPI\UploadXmlService\UploadedFiles\193.json";
        private static readonly string fileLocation = @"C:\Repo-1\211.json";


        //public async Task<XmlFileStorage> Storexmltodatabase()
        //{

        //    string fileName = Path.GetFileName(fileLocation);
        //    string contentOfFile = File.ReadAllText(fileLocation);
        //    DateTime dateTime = DateTime.Now;
        //    //int fileId;

        //    var query = "insert into[splitXML].[dbo].[FileStorage](FileNames, FileContent, FileCreationDate) Values(@fileName,@contentOfFile,@dateTime)";

        //    var parameters = new DynamicParameters();

        //    parameters.Add("fileName", fileName, DbType.String);
        //    parameters.Add("contentOfFile", contentOfFile, DbType.String);
        //    parameters.Add("dateTime", dateTime, DbType.DateTime);


        //    // Used to Paste the table row in the database.
        //    using (var connection = _dapperContext.CreateConnection())
        //    {
        //       var stored =  await connection.ExecuteAsync(query, parameters);
        //    }

        //    //var queryId = @"SELECT id FROM [splitXML].[dbo].[FileStorage] where FileNames = ('{fileName}') ";


        //    //using (var connection = _dapperContext.CreateConnection())
        //    //{
        //    ////    fileId = connection.Execute(queryId);
        //    //}

        //    var finalResult = new XmlFileStorage
        //    {
        //        FileNames = fileName,
        //        FileContent = contentOfFile,
        //        FileCreationDate = dateTime
        //    };

        //    return finalResult;

        //}





        public async Task<FileStorage> StoreFiletodatabase()
        {
            var resultKafka = KafkaConsume();

            string fileName = Path.GetFileName(fileLocation);
            string contentOfFile = File.ReadAllText(fileLocation);
            DateTime dateTime = DateTime.Now;

            // table name here mentioned.
            var query = "insert into[splitXML].[dbo].[FileStorage](FileNames, FileContent, FileCreationDate) Values(@fileName,@contentOfFile,@dateTime)";

            var parameters = new DynamicParameters();

            parameters.Add("fileName", fileName, DbType.String);
            parameters.Add("contentOfFile", contentOfFile, DbType.String);
            parameters.Add("dateTime", dateTime, DbType.DateTime);


            // Used to Paste the table row in the database.
            using (var connection = _dapperContext.CreateConnection())
            {
                var stored = await connection.ExecuteAsync(query, parameters);

                ProduceIntoKafka(fileName);
                return null;
            }



        }

        private string KafkaConsume()
        {
            using (var consumer = new ConsumerBuilder<Null, string>(_config).Build())
            {
                consumer.Subscribe("file_upload");
                var consumerResult = consumer.Consume();
                return consumerResult.Message.Value;
            }
        }
        public async Task<string> ProduceIntoKafka(string fileName)
        {

            //string serializedFileContent = JsonConvert.SerializeObject(contentOfFile);

            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                var producedMessage = await producer.ProduceAsync("file_read", new Message<Null, string> { Value = fileName + " successfully saved into DB" });
                return null;
            }
        }

        public async Task<FileStorage> GetFiledataFromdatabase(int Id)
        {
            var getRowString = "SELECT [id],[fileNames],[fileContent],[fileCreationDate] FROM [splitXML].[dbo].[FileStorage] where id = @Id ";

            var parameters = new DynamicParameters();
            parameters.Add("id", Id, DbType.Int32);

            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    var stored = await connection.QueryFirstOrDefaultAsync<FileStorage>(getRowString, parameters);
                    return stored;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
