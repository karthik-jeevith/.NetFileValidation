using System;
using System.Collections.Generic;
using System.Text;

namespace UploadXmlAPI.Model
{
    public class XmlFileUpload
    {
        public int Id { get; set; }
        public string FileNames { get; set; }

        public string FileContent { get; set; }

        public DateTime FileCreationDate { get; set; }

    }
}
