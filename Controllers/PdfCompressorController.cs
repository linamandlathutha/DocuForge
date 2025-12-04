using Microsoft.AspNetCore.Mvc;
using PdfCompressor.Models;
using System.Diagnostics;


namespace PdfCompressor.Controllers
{
    public class PdfCompressorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Compress(PdfUploadViewModel model)
        {
            if (model.PdfFile == null || model.PdfFile.Length == 0)
            {
                ViewBag.Message = "Please upload a valid PDF file.";
                return View("Index");
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsPath);

            var inputPath = Path.Combine(uploadsPath, model.PdfFile.FileName);
            var outputPath = Path.Combine(uploadsPath, "compressed_" + model.PdfFile.FileName);

            using (var stream = new FileStream(inputPath, FileMode.Create))
            {
                await model.PdfFile.CopyToAsync(stream);
            }

       
            var gsPath = @"C:\Program Files\gs\gs10.06.0\bin\gswin64c.exe"; 
            var args = $"-sDEVICE=pdfwrite -dCompatibilityLevel=1.4 -dPDFSETTINGS=/screen " +
                       $"-dNOPAUSE -dQUIET -dBATCH -sOutputFile=\"{outputPath}\" \"{inputPath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gsPath,
                    Arguments = args,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (!System.IO.File.Exists(outputPath))
            {
                ViewBag.Message = "Compression failed. Check Ghostscript path.";
                return View("Index");
            }

            var compressedBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
            return File(compressedBytes, "application/pdf", Path.GetFileName(outputPath));
        }
    }
}
