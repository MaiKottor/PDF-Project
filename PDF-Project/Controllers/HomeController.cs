using Microsoft.AspNetCore.Mvc;
using PDF_Project.Models;
using PDF_Project.Services;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PDF_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly PdfProcessor _pdfProcessor;

        public HomeController()
        {
            _pdfProcessor = new PdfProcessor(); // Initialize PdfProcessor
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Render the form to the user
        }

        [HttpPost]
        public IActionResult GenerateReport(CLIInput input)
        {
            if (string.IsNullOrWhiteSpace(input.InputFilePath) || string.IsNullOrWhiteSpace(input.OutputFilePath))
            {
                ViewBag.Error = "Both input and output paths are required.";
                return View("Index");
            }

            List<string> filePaths;

            try
            {
                // Read file paths from the input file and validate them
              
                 filePaths = input.InputFilePath.Select(path => Path.Combine(Directory.GetCurrentDirectory(), input.InputFilePath)).ToList();

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error reading input file: {ex.Message}";
                return View("Index");
            }

            if (!filePaths.Any())
            {
                ViewBag.Error = "No valid PDF files found in the input file.";
                return View("Index");
            }

            List<PdfMetadata> metadataList;

            try
            {
                // Use PdfProcessor to process files in parallel
                metadataList = _pdfProcessor.ProcessFilesParallel(filePaths);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing files: {ex.Message}";
                return View("Index");
            }

            // Analyze metadata
            var report = new
            {
                ProcessedFiles = metadataList.Select(m => m.FilePath).Distinct().ToList(),
                FilesWithMissingMetadata = metadataList.Where(m => string.IsNullOrWhiteSpace(m.Title) ||
                                                                   string.IsNullOrWhiteSpace(m.Author) ||
                                                                   string.IsNullOrWhiteSpace(m.CreationDate))
                                                       .Select(m => m.FilePath).Distinct().ToList(),
                TotalPages = metadataList.Sum(m => m.NumberOfPages)
            };

            try
            {
                // Save the report as a JSON file
                System.IO.File.WriteAllText(input.OutputFilePath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
                ViewBag.Message = "Report generated successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error saving the report: {ex.Message}";
                return View("Index");
            }

            return View("Index");
        }

    }
}
