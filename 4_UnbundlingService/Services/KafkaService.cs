using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUnbundlingService
{
    public class KafkaService
    {
        private readonly ProducerConfig _producerconfig;

        public KafkaService(ProducerConfig producerconfig)
        {
            _producerconfig = producerconfig;
        }

        public  async Task<string> KafkaProduceMethod()
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerconfig).Build())
            {
                var producedMessage = await producer.ProduceAsync("Payments", new Message<Null, string> { Value = "Message got uploaded in database" });
                return  producedMessage.Message.Value;
            }
        }
    }
}
