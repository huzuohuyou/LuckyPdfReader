using System;

namespace LuckyPdfReader.Application.Models
{
    public class Token
    {
        public string default_sbox_drive_id { get; set; }
        public string role { get; set; }
        public string user_name { get; set; }
        public bool need_link { get; set; }
        public DateTime expire_time { get; set; }
        public bool pin_setup { get; set; }
        public bool need_rp_verify { get; set; }
        public string avatar { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public string default_drive_id { get; set; }
        public string domain_id { get; set; }
        public string refresh_token { get; set; }
        public bool is_first_login { get; set; }
        public string user_id { get; set; }
        public string nick_name { get; set; }
        public object[] exist_link { get; set; }
        public string state { get; set; }
        public int expires_in { get; set; }
        public string status { get; set; }
    }

   
}
