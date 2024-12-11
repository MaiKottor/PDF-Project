using PDF_Project.Models;
using System.Collections.Concurrent;
using UglyToad.PdfPig;

namespace PDF_Project.Services
{
	
		public class PdfProcessor
		{
			public PdfMetadata ExtractMetadata(string filePath)
			{
				using var pdf = PdfDocument.Open(filePath);
				var information = pdf.Information;

				return new PdfMetadata
				{
					FilePath = filePath,
					Title = information.Title ?? "Unknown Title",
					Author = information.Author ?? "Unknown Author",
					CreationDate = information.CreationDate ?? DateTime.MinValue.ToString(),
					NumberOfPages = pdf.NumberOfPages
				};
			}

			public List<PdfMetadata> ProcessFilesParallel(IEnumerable<string> filePaths)
			{
				var metadataList = new ConcurrentBag<PdfMetadata>();

				Parallel.ForEach(filePaths, path =>
				{
					var metadata = ExtractMetadata(path);
					metadataList.Add(metadata);
				});

				return metadataList.ToList();
			}
		}
	
}
