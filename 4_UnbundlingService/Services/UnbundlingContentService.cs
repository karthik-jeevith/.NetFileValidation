using Confluent.Kafka;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnbundlingService.Dapper;
using UnbundlingService.Interfaces;
using UnbundlingService.Models;

namespace UnbundlingService.Services
{
    public class UnbundlingContentService : IUnbundlingContentService
    {
        private readonly ConsumerConfig _consumerconfig;
        private readonly ProducerConfig _producerconfig;
        private readonly DapperContext _dapperContext;


        public UnbundlingContentService(ConsumerConfig config, DapperContext dapperContext, ProducerConfig producerconfig)
        {
            _consumerconfig = config;
            _dapperContext = dapperContext;
            _producerconfig = producerconfig;
        }

        public string KafkaConsumeMethod()
        {
            using (var consumer = new ConsumerBuilder<Null, string>(_consumerconfig).Build())
            {
                consumer.Subscribe("ValidFile");
                while (true)
                {
                    var consumerResult = consumer.Consume();
                    return consumerResult.Message.Value;
                }
            }
        }

        public string UnbundlingMethod(string fileContent)
        {
            var finalString = string.Empty;

            var result = JsonConvert.DeserializeObject<JsonCCOE>(fileContent);

            for (int i = 0; i < result.Messages.Count; i++)
            {
                List<Messages> ListOfEachMessages = result.Messages;
                finalString = JsonConvert.SerializeObject(ListOfEachMessages[i]);
                var EachMessageResult = StoreEachMessageToDatabase(finalString);
            }
            return finalString;

        }

        public async Task<bool> StoreEachMessageToDatabase(string JsonString)
        {

            try
            {
                // Here we have mentioned the table name. so, same connection string will be used in dapper database.
                var query = "insert into [splitXML].[dbo].[Unbundling] (Messages) Values(@Messages)";

                var parameters = new DynamicParameters();

                parameters.Add("Messages", JsonString, DbType.String);

                // Used to Paste the table row in the database.
                using (var connection = _dapperContext.CreateConnection())
                {
                    var stored = await connection.ExecuteAsync(query, parameters);
                    var result = KafkaProduceMethod();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<string> KafkaProduceMethod()
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerconfig).Build())
            {
                var producedMessage = await producer.ProduceAsync("Payments", new Message<Null, string> { Value = "Message got uploaded in database" });
                return producedMessage.Message.Value;
            }
        }
    }
}
