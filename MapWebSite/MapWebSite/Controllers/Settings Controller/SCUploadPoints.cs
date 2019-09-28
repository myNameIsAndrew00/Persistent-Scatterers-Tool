using MapWebSite.HtmlHelpers;
using MapWebSite.Interaction;
using MapWebSite.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// Use this ApiController to return pages for the settings layer and to interact with it
    /// </summary>
    //[Filters.ApiAuthenticationFilter]
    [Authorize]
    public partial class SettingsController : ApiController
    {

        /// <summary>
        /// Upload part of a file to the server. 
        /// The chunk will be saved acording to its user and its dataset name on disk. Chunk names will be indexes (0,1,2,...n).
        /// After more chunks will be sent, call the Merge method to rebuild the initial file using their indexes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UploadFileChunk()
        {
            string directoryName = $"{ConfigurationManager.AppSettings["PointsDatasetsCheckpointFolder"]}\\{RouteConfig.CurrentUser.Username}";
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            foreach (string file in HttpContext.Current.Request.Files)
            {
                var FileData = HttpContext.Current.Request.Files[file];

                string[] checkoutData = FileData.FileName.Split(new char[] { '_' }, 2, StringSplitOptions.RemoveEmptyEntries);

                string fileFolderName = $"{directoryName}\\{checkoutData[0]}";

                if (!Directory.Exists(fileFolderName)) Directory.CreateDirectory(fileFolderName);

                if (FileData?.ContentLength > 0)
                {
                    using (var fileStream = File.Create($"{fileFolderName}\\{checkoutData[1]}"))
                        FileData.InputStream.CopyTo(fileStream);
                }

            }

            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("Chunk uploaded")
            };
        }



        [HttpPost]
        public HttpResponseMessage ClearFileChunks()
        {
            //TODO: implement
            throw new NotImplementedException();
        }


        [HttpPost]
        public HttpResponseMessage MergeFileChunks([FromBody] JObject data)
        {
            string fileName = data["fileName"].ToObject<string>();
            if(string.IsNullOrEmpty(fileName))
                return new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError };

            string directoryName = $"{ConfigurationManager.AppSettings["PointsDatasetsCheckpointFolder"]}\\{RouteConfig.CurrentUser.UserName}\\{fileName}";

            if (!Directory.Exists(directoryName))
                return new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError };

            var chunksFiles = Directory.GetFiles(directoryName).OrderBy(file => file);

            using (FileStream finalFile = new FileStream($"{directoryName}\\__OK", FileMode.Create))
                foreach (var chunkFile in chunksFiles)
                {
                    using (FileStream fileChunk =
                               new FileStream(chunkFile, FileMode.Open))
                    {
                        fileChunk.CopyTo(finalFile);
                    }
                }

            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("File merged")
            };
        }

        [HttpPost]
        public HttpResponseMessage CheckDatasetExistance([FromBody] JObject data)
        {                     
            string directoryName = $"{ConfigurationManager.AppSettings["PointsDatasetsCheckpointFolder"]}\\{RouteConfig.CurrentUser.UserName}\\{data["fileName"].ToObject<string>()}";

            bool directoryExists = Directory.Exists(directoryName);

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(MessageBoxBuilder.Create(!directoryExists ? "Success" : "Can't finish operation",
                                                                          !directoryExists ? "Success"
                                                                                         : "You chosed an existent name for your dataset. Please chose another"))
            };                 
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

    }
}
