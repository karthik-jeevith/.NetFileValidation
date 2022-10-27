using FileReadFromDb.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FileReadFromDb
{
    public interface IJsonValidatorService
    {
        public Task<bool> JsonValidatorMethod(string contentOfFile, string fileName);
        public  Task<bool> StoreValidationResultToDatabase(string IsValid, string IsInValid, string FileName);
        public Task<bool> ProduceTheResultIntoKafka(string fileName, string topicName, string contentOfFile);
        public Task<string> ConsumeTheResultFromKafka();


    }
}
