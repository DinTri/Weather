namespace Trifon.IDP.EmailService.Dtos
{
    public class EmailDto
    {
        public string To { get; set; } = string.Empty;
        public string ToWhom { get; set; } = string.Empty;
        public string From { get; set; } = String.Empty;
        public string FromWhom { get; set; } = String.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
