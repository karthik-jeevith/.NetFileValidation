using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreXmlDbService.Model;

namespace StoreXmlDbService.DapperFolder
{
    public interface IFileRepository
    {
        public Task<FileStorage> StoreFiletodatabase();
        public Task<FileStorage> GetFiledataFromdatabase(int Id);
    }
}
