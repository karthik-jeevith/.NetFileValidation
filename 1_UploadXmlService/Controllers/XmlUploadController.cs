using Confluent.Kafka;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadXmlAPI.Model;

namespace splitXmlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XmlUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ProducerConfig _config;

        public XmlUploadController(ProducerConfig config,IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        // *******************Single File Upload with Kafka-producer ************************************

        [HttpPost("AddSingleFileToDb")]
        public async Task<ActionResult> AddSingleXMl(/*string topic,*/ IFormFile file)
        {

            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");
            string filepath = Path.Combine(directoryPath, file.FileName);

            // paste the content of uploaded file to the file path
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            //the properties for the object
            string fileName = Path.GetFileName(filepath);
            string contentOfFile = System.IO.File.ReadAllText(filepath);
            DateTime dateTime = DateTime.Now;


            //// creating object for the model class XmlFileUpload to be sent to kafaka-producer
            //var fileProperties = new XmlFileUpload
            //{
            //    FileCreationDate = dateTime,
            //    FileNames = fileName
            //};

            //string serializedEmployee = JsonConvert.SerializeObject(fileProperties);

            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
               var producedMessage =  await producer.ProduceAsync("file_upload", new Message<Null, string> { Value = fileName + " successfully upload to local folder" });
                Console.WriteLine(producedMessage.Message.Value);

                //producer.Flush(TimeSpan.FromSeconds(20));
                //return Ok(true);
            }

            return Ok("File Upload Successful");
        }


        // *******************Multiple Files Upload with kafka-producer ************************************

        [HttpPost("AddMultipleFilesToDb")]
        public async Task<ActionResult> AddMultipleXMl(string topic, List<IFormFile> files)
        {
            if (files.Count == 0)
            {
                return BadRequest();
            }

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            try
            {
                foreach (var file in files)
                {
                    string filepath = Path.Combine(directoryPath, file.FileName);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    //the properties for the object to be produced.
                    string fileName = Path.GetFileName(filepath);
                    string contentOfFile = System.IO.File.ReadAllText(filepath);
                    DateTime dateTime = DateTime.Now;

                    // creating object for the model class XmlFileUpload to be sent to kafaka-producer
                    var fileProperties = new XmlFileUpload
                    {
                        FileContent = contentOfFile,
                        FileCreationDate = dateTime,
                        FileNames = fileName
                    };

                    string serializedFileProperties = JsonConvert.SerializeObject(fileProperties);

                    using (var producer = new ProducerBuilder<Null, string>(_config).Build())
                    {
                        await producer.ProduceAsync(topic, new Message<Null, string> { Value = fileName + " successfully upload to local folder" });
                        //producer.Flush(TimeSpan.FromSeconds(20));
                        //return Ok(true);
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }


            return Ok("upload multiple files was successful");
        }


    }


}
