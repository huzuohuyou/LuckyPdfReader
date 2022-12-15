using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace aliyundrive_Client_CSharp.aliyundrive
{
    // HttpClient源码 https://github.com/dotnet/corefx/blob/d69d441dfb0710c2a34155c7c4745db357b14c96/src/System.Net.Http/src/System/Net/Http/HttpClient.cs 
    class Hclient<T> : HttpClient where T : MagBase
    {
        public Hclient() : base()//new HclientProcessing()
        {
        }
        public async Task<HttpResult<T>> GetAsJsonAsync(string url)
        {
            var r = new HttpResult<T>();
            try
            {
                var response = await GetAsync(url);
                r.success = response.IsSuccessStatusCode;
                r.body = await response.Content.ReadAsStringAsync();
                r.obj = JsonConvert.DeserializeObject<T>(r.body);
                r.code = r.obj.code;
                r.message = r.obj.message;
                r.ok = true;
            }
            catch (Exception ex)
            {
                r.err = ex.Error();
            }
            return r;
        }
        public async Task<HttpResult<T>> PostAsJsonAsync(string url, object data, TaskInfo task=null, bool redirect = false,string token= "")
        {
            token =
                "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI4MTlhMGM3ZWYyNTk0MzhhOTc2MTA4YjNlMDBiNmIyMiIsImN1c3RvbUpzb24iOiJ7XCJjbGllbnRJZFwiOlwiMjVkelgzdmJZcWt0Vnh5WFwiLFwiZG9tYWluSWRcIjpcImJqMjlcIixcInNjb3BlXCI6W1wiRFJJVkUuQUxMXCIsXCJTSEFSRS5BTExcIixcIkZJTEUuQUxMXCIsXCJVU0VSLkFMTFwiLFwiVklFVy5BTExcIixcIlNUT1JBR0UuQUxMXCIsXCJTVE9SQUdFRklMRS5MSVNUXCIsXCJCQVRDSFwiLFwiT0FVVEguQUxMXCIsXCJJTUFHRS5BTExcIixcIklOVklURS5BTExcIixcIkFDQ09VTlQuQUxMXCIsXCJTWU5DTUFQUElORy5MSVNUXCIsXCJTWU5DTUFQUElORy5ERUxFVEVcIl0sXCJyb2xlXCI6XCJ1c2VyXCIsXCJyZWZcIjpcImh0dHBzOi8vd3d3LmFsaXl1bmRyaXZlLmNvbS9cIixcImRldmljZV9pZFwiOlwiMmMxYmJhODRlYzI3NGU0MGFjZjhjYTM5YzY2ZThmMDZcIn0iLCJleHAiOjE2NzA3Njk1ODcsImlhdCI6MTY3MDc2MjMyN30.pEVxAM8meQJX9ECkSA-zZGm8Ks3I73ZvQfVLF2fE3vWx0LdYsDl9siu97fQchL8zAqaUaJxwFz7sBuXIEXCO7p4ZABObTTZAYKZlb_PvSBwuz8mp9aSvH5UbCCHmtC7JMaN7nitGKykwcIeZDQEBBCQq60tgQPkDDRCsRiIZd9A";
            var c = new HclientContent(JsonConvert.SerializeObject(data), task);
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var r = new HttpResult<T>();
            try
            {
                var response = await PostAsync(url, c);
                r.success = response.IsSuccessStatusCode;
                r.body = await response.Content.ReadAsStringAsync();
                r.obj = JsonConvert.DeserializeObject<T>(r.body);
                r.code = r.obj.code;
                r.message = r.obj.message;
                r.ok = true;
                if (r.code == "AccessTokenInvalid")
                {
                    Console.WriteLine($"AccessToken过期[{redirect}]");
                    if (!redirect)
                    {
                        var re = await Token.Instance.Refresh();
                        if (re.Yes) return await PostAsJsonAsync(url, data, task, true,token);
                    }
                }
            }
            catch (Exception ex)
            {
                r.err = ex.Error();
            }
            return r;
        }
        public async Task<HttpResult<T>> PutAsJsonAsync(string url, Stream stream, TaskInfo task)
        {
            var r = new HttpResult<T>();
            var c = new HclientContent(stream, task);
            try
            {
                var response = await PutAsync(url, c, task.cancelToken);
                r.success = response.IsSuccessStatusCode;
                r.ok = true;
                if (r.success) r.body = response.Headers.ETag.Tag;
                else
                {
                    r.body = await response.Content.ReadAsStringAsync();
                    var xml = new XmlDocument();
                    xml.LoadXml(r.body);
                    var err = 
                    r.code = xml.SelectSingleNode("/Error/Code").InnerText;
                    r.message = xml.SelectSingleNode("/Error/Message").InnerText;
                }
            }
            catch (Exception ex)
            {
                r.ok = false;
                r.err = ex.Error();
            }
            return r;
        }

    }


    public class HclientProcessing : MessageProcessingHandler
    {
        public HclientProcessing()
        {
            InnerHandler = new HttpClientHandler();
        }
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return request;
        }
        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return response;
        }
    }

    class HclientContent : HttpContent
    {
        private readonly Stream _innerContent;
        private readonly TaskInfo _task;
        private readonly long StreamLength;
        //private readonly ManualResetEvent _manualResetEvent;
        public HclientContent(Stream innerContent, TaskInfo task)
        {
             _innerContent = innerContent;
            if (task == null)
            {
                StreamLength = _innerContent.Length;
            }
            else
            {
                _task = task;
                StreamLength = task.Length > 0 ? task.Length : _innerContent.Length;
                if (task.Position > 0) _innerContent.Position = task.Position;
            }
            Headers.ContentLength = StreamLength;
        }

        public HclientContent(Stream innerContent) : this(innerContent, null)
        {
        }
        public HclientContent(byte[] bytes) : this(new MemoryStream(bytes))
        {
        }
        public HclientContent(byte[] bytes, TaskInfo task) : this(new MemoryStream(bytes), task)
        {
        }
        public HclientContent(string content) : this(content,null)
        {
        }
        public HclientContent(string content,TaskInfo task) : this(Encoding.UTF8.GetBytes(content),task)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Run(() =>
            {
                bool taskrun = _task != null;
                long chunk = 0;
                long chunkLength = StreamLength;
                if (taskrun) chunkLength = StreamLength / 20;
                if (chunkLength == 0) chunkLength = StreamLength;
                long step = chunkLength;
                var buffer = new byte[chunkLength];
                using (_innerContent) while (true)
                    {
                        if (StreamLength - chunk < chunkLength) step = StreamLength - chunk;
                         var length = _innerContent.Read(buffer, 0, (int)step);
                        if (length <= 0) break;
                        stream.Write(buffer, 0, length);
                        if (taskrun)
                        {
                            chunk += length;
                            _task.cancelToken.ThrowIfCancellationRequested();
                            _task.Step = (int)(100 * chunk / StreamLength);
                        }
                    }
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length= _innerContent.Length;
            return true;
        }
    }
}
