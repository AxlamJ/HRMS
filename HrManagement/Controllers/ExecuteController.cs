using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Controllers
{
    public class ExecuteController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ExecuteController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public ActionResult Files(int count = 100, String FolderName = "Logs")
        {
            List<FileInfo> sortedFiles = DirSearch(Path.Combine( _env.ContentRootPath, FolderName))
                                                  .OrderByDescending(f => f.LastWriteTime)
                                                  .ToList();
            List<FileInfo> sortedFilesUpdated = new List<FileInfo>();

            count = count >= sortedFiles.Count ? sortedFiles.Count : count;


            for (int i = 0; i < count; i++)
            {
                sortedFilesUpdated.Add(sortedFiles[i]);
            }

            List<String> FilePaths = new List<string>();
            List<ExecuteFiles> Files = new List<ExecuteFiles>();
            for (int i = 0; i < sortedFilesUpdated.Count; i++)
            {
                ExecuteFiles model = new ExecuteFiles();
                model.FileName = sortedFilesUpdated[i].FullName.Replace(_env.WebRootPath, "");
                model.Date = sortedFilesUpdated[i].CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                Files.Add(model);

                //FilePaths.Add(sortedFilesUpdated[i].FullName.Replace(System.Web.HttpContext.Current.Server.MapPath("~/"), ""));
            }


            return View(Files);
        }


        public ActionResult ShowText(string Path)
        {

            String Text = System.IO.File.ReadAllText(Path);


            ExecuteShowText ExecuteShowText = new ExecuteShowText();
            ExecuteShowText.Data = Text;

            return View(ExecuteShowText);
        }


        private List<FileInfo> DirSearch(string sDir)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo f in new DirectoryInfo(sDir).GetFiles())
            {
                files.Add(f);
            }
            foreach (string d in Directory.GetDirectories(sDir))
            {
                files.AddRange(DirSearch(d));
            }
            return files;
        }

    }
    public class ExecuteFiles
    {
        public string FileName { get; set; }
        public string Date { get; set; }
    }

    public class ExecuteShowText
    {
        public string Data { get; set; }
    }
}
