namespace Api.Models
{
    public class CommentFile
    {
        public int Id { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public int CommentId { get; set; }
        public Comment? Comment { get; set; }
    }
}
