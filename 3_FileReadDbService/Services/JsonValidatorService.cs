
using Confluent.Kafka;
using Dapper;
using FileReadFromDb.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileReadFromDb.Services
{
    public class JsonValidatorService : IJsonValidatorService
    {
        private readonly DapperContext _dapperContext;
        private readonly ProducerConfig _producerConfig;
        private readonly ConsumerConfig _consumerconfig;

        private readonly string Validtopic = "ValidFile";
        private readonly string InValidtopic = "InValidFile";


        public JsonValidatorService(DapperContext dapperContext, ProducerConfig producerconfig, ConsumerConfig consumerconfig)
        {
            _dapperContext = dapperContext;
            _producerConfig = producerconfig;
            _consumerconfig = consumerconfig;
        }

        public async Task<bool> JsonValidatorMethod(string contentOfFile, string fileName)
        {

            try
            {
                JSchema schemaResult = JSchema.Parse(File.ReadAllText("jsonSchema.json"));

                JObject jsonObject = JObject.Parse(contentOfFile);

                IList<string> validationEvents = new List<string>();

                if (jsonObject.IsValid(schemaResult, out validationEvents))
                {
                    // save "valid" into db.

                    string IsValid = "true";
                    string IsInValid = "false";

                    // calling the method to store the valid result.
                    var ValidationResult = await StoreValidationResultToDatabase(IsValid, IsInValid, fileName);

                    if (ValidationResult)
                    {
                        var result = await ProduceTheResultIntoKafka(fileName, Validtopic, contentOfFile);
                    }
                    return true;
                }

                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                // save "invalid" into db.

                string IsValid = "false";
                string IsInValid = "true";

                // calling the method to store the valid result.
                var ValidationResult = StoreValidationResultToDatabase(IsValid, IsInValid, fileName);

                var result = await ProduceTheResultIntoKafka(fileName, InValidtopic, contentOfFile);

                //should produce into Kafka-invalid Topic.
                return false;
            }

        }


        public async Task<bool> StoreValidationResultToDatabase(string IsValid, string IsInValid, string FileName)
        {

            try
            {
                // Here we have mentioned the table name. so, same connection string will be used in dapper database.
                var query = "insert into [splitXML].[dbo].[FileValidation](FileName, Valid, Invalid) Values(@FileName,@IsValid,@IsInValid)";

                var parameters = new DynamicParameters();

                parameters.Add("FileName", FileName, DbType.String);
                parameters.Add("IsValid", IsValid, DbType.String);
                parameters.Add("IsInValid", IsInValid, DbType.String);


                // Used to Paste the table row in the database.
                using (var connection = _dapperContext.CreateConnection())
                {
                    var stored = await connection.ExecuteAsync(query, parameters);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> ProduceTheResultIntoKafka(string fileName, string topicName, string contentOfFile)
        {
            string kafkaMessage=string.Empty;

            if (topicName == "ValidFile")
            {
                kafkaMessage = " is a valid file.";
            }
            else
            {
                kafkaMessage = " is NOT a valid file.";
            }


            //string serializedFileContent = JsonConvert.SerializeObject(contentOfFile);

            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                //var producedMessage = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = fileName + kafkaMessage});
                var producedMessageContent = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = contentOfFile });

                Console.WriteLine(producedMessageContent.Message.Value);

                return true;

                //producer.Flush(TimeSpan.FromSeconds(20));
                //return Ok(true);
            }
        }

        public Task<string> ConsumeTheResultFromKafka()
        {
            using (var consumer = new ConsumerBuilder<Null, string>(_consumerconfig).Build())
            {
                consumer.Subscribe("file_read");
                var consumerResult = consumer.Consume();
                return Task.FromResult(consumerResult.Message.Value);
            }
        }



    }
}


