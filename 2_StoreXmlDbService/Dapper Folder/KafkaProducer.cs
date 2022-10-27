using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreFileDbService
{
    public class KafkaProducer
    {
        private readonly ProducerConfig _config;

        public KafkaProducer(ProducerConfig config)
        {
            _config = config;
        }


        public async Task<string> ProduceTheResultIntoKafka(string fileName)
        {

            //string serializedFileContent = JsonConvert.SerializeObject(contentOfFile);

            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                var producedMessage = await producer.ProduceAsync("first_read", new Message<Null, string> { Value = fileName + " successfully saved into DB" });
                return null;
            }
        }
    
    }
}
