using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreXmlDbService.DapperFolder;
using StoreXmlDbService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.dapper_folder
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileStoreController : ControllerBase
    {
        private readonly IFileRepository _xmlRepo;

        public FileStoreController(IFileRepository xmlRepo)
        {
            _xmlRepo = xmlRepo;
        }

        [HttpPost("SaveFileDataIntoDb")]
        public async Task<FileStorage> StoreInDbMethod()
        {
            var result = await _xmlRepo.StoreFiletodatabase();
            return result;
        }

        //[EnableCors("AllowOrigin")]
        [HttpGet("GetFileData/{id}")]
        public async Task<FileStorage> GetFileMethod(int id)
        {
            var result = await _xmlRepo.GetFiledataFromdatabase(id);
            return result;
        }
    }
}
