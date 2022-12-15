using System;
using System.Collections.Generic;
using System.Text;

namespace LuckyPdfReader.Application.Models
{
    public class DownloadFile
    {
        public string domain_id { get; set; }
        public string drive_id { get; set; }
        public string file_id { get; set; }
        public string revision_id { get; set; }
        public string method { get; set; }
        public string url { get; set; }
        public string internal_url { get; set; }
        public DateTime expiration { get; set; }
        public int size { get; set; }
        public string crc64_hash { get; set; }
        public string content_hash { get; set; }
        public string content_hash_name { get; set; }
    }

}
