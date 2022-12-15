namespace aliyundrive_Client_CSharp.aliyundrive
{

    public class ListParam
    {
        public string drive_id { get; set; } = "33864490";
        public string parent_file_id { get; set; } = "root";
        public int limit { get; set; } = 20;
        public bool all { get; set; } = false;
        public int url_expire_sec { get; set; } = 14400;
        public string image_thumbnail_process { get; set; } = "image/resize,w_256/format,jpeg";
        public string image_url_process { get; set; } = "image/resize,w_1920/format,jpeg/interlace,1";
        public string video_thumbnail_process { get; set; } = "video/snapshot,t_1000,f_jpg,ar_auto,w_256";
        public string fields { get; set; } = "*";
        public string order_by { get; set; } = "updated_at";
        public string order_direction { get; set; } = "DESC";
    }

}
