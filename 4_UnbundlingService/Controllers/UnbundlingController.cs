using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnbundlingService.Interfaces;
using UnbundlingService.Services;

namespace UnbundlingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnbundlingController : ControllerBase
    {
        private readonly IUnbundlingContentService _unbundlingContentService;

        public UnbundlingController(IUnbundlingContentService unbundlingContentService)
        {
            _unbundlingContentService = unbundlingContentService;
        }

        [HttpPost("UnbundlingTheFileContent")]
        public Task<string> UnBundlingMethod()
        {
            var kafkaFileContent = _unbundlingContentService.KafkaConsumeMethod();
            var result = _unbundlingContentService.UnbundlingMethod(kafkaFileContent);
            return Task.FromResult(result);
        }



    }
}
