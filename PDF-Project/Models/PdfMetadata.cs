namespace PDF_Project.Models
{
	public class PdfMetadata
	{
		public string FilePath { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public DateTime? CreationDate { get; set; }
		public int NumberOfPages { get; set; }
		public bool IsMetadataComplete => !string.IsNullOrWhiteSpace(Title) &&
										  !string.IsNullOrWhiteSpace(Author) &&
										  CreationDate.HasValue;
	}
}
