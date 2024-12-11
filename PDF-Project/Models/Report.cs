namespace PDF_Project.Models
{
	public class Report
	{
		public List<string> ProcessedFiles { get; set; } = new();
		public List<string> FilesWithMissingMetadata { get; set; } = new();
		public int TotalPages { get; set; }
	}
}
