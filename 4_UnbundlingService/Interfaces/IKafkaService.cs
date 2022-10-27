using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUnbundlingService
{
    public interface IKafkaService
    {
        public string KafkaConsumeMethod();
    }
}
