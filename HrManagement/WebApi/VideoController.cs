using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.WebApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public VideoController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("{Url}")]
        public IActionResult GetVideo(string Url)
        {
            // Assuming videos stored in wwwroot/videos
            var videoPath = Path.Combine(_env.WebRootPath, "videos/Lessons", Url);

            if (!System.IO.File.Exists(videoPath))
                return NotFound();

            var fileStream = System.IO.File.OpenRead(videoPath);
            var fileLength = fileStream.Length;
            var contentType = "video/mp4";
            var rangeHeader = Request.Headers["Range"].ToString();

            if (string.IsNullOrEmpty(rangeHeader))
            {
                return File(fileStream, contentType, enableRangeProcessing: true);
            }
            else
            {
                var range = rangeHeader.Replace("bytes=", "").Split('-');
                var start = long.Parse(range[0]);
                var end = range.Length > 1 && !string.IsNullOrEmpty(range[1]) ? long.Parse(range[1]) : fileLength - 1;
                var length = end - start + 1;

                var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                stream.Position = start;

                var buffer = new byte[length];
                stream.Read(buffer, 0, (int)length);
                Response.StatusCode = 206; // Partial Content
                Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileLength}");
                Response.Headers.Add("Accept-Ranges", "bytes");
                Response.Headers.Add("Content-Length", length.ToString());

                return File(new MemoryStream(buffer), contentType, enableRangeProcessing: false);
            }
        }
    }

}
