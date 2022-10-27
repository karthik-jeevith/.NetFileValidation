using FileReadFromDb.Model;
using FileReadFromDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FileReadFromDb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileValidateController : ControllerBase
    {
        private readonly IJsonValidatorService _xmlRepo;

        public FileValidateController(IJsonValidatorService xmlRepo)
        {
            _xmlRepo = xmlRepo;
        }

        private string BaseUrl = "https://localhost:44384/api/FileStore/GetFileData/";

       // This HttpClient is calling the GET method that is in the MicroService-2.
        [Produces("application/json")]
        [HttpGet("{Id}")]
        public async Task<FileStorage> GetFileDataRemote(int Id)
        {
            try
            {
                var ConsumeResult = _xmlRepo.ConsumeTheResultFromKafka();

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage httpResponse = await client.GetAsync(BaseUrl + Id);

                if (httpResponse.IsSuccessStatusCode)
                {           
                    FileStorage xmlFileStorage = await httpResponse.Content.ReadAsAsync<FileStorage>();

                    // validate the content of the JSON which is fetched through GET HTTPClient.
                    var result = _xmlRepo.JsonValidatorMethod(xmlFileStorage.FileContent, xmlFileStorage.FileNames);

                    return xmlFileStorage;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
