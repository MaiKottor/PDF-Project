using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDF_Project.Models;
using PDF_Project.Services;
using System.Text.Json;

namespace PDF_Project.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportController : ControllerBase
	{
		private readonly PdfProcessor _pdfProcessor;

		public ReportController()
		{
			_pdfProcessor = new PdfProcessor();
		}

		[HttpPost("generate")]
		public IActionResult GenerateReport([FromBody] ReportRequest request)
		{
			if (request == null || !request.FilePaths.Any())
			{
				return BadRequest("No file paths provided.");
			}

			// Convert relative paths to absolute paths
			var absolutePaths = request.FilePaths.Select(path => Path.Combine(Directory.GetCurrentDirectory(), path)).ToList();

			var metadataList = _pdfProcessor.ProcessFilesParallel(absolutePaths);

			// Analyze metadata
			var report = AnalyzeMetadata(metadataList);

			// Save the report
			try
			{
				var outputPath = Path.Combine(Directory.GetCurrentDirectory(), request.OutputPath);
				var jsonReport = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
				System.IO.File.WriteAllText(outputPath, jsonReport);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error saving the report: {ex.Message}");
			}

			return Ok(new { message = "Report generated successfully.", report });
		}
		private Report AnalyzeMetadata(List<PdfMetadata> metadataList)
		{
			var report = new Report();

			foreach (var metadata in metadataList)
			{
				report.ProcessedFiles.Add(metadata.FilePath);

				if (!metadata.IsMetadataComplete)
				{
					report.FilesWithMissingMetadata.Add(metadata.FilePath);
				}

				report.TotalPages += metadata.NumberOfPages;
			}

			return report;
		}
	
}
}
