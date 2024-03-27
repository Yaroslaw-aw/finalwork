namespace ApiMailServer.Dto
{
    public class MessagesSentDto
    {
        public Guid ProducerId { get; set; }
        public string? ProducerEmail { get; set; }
        public string? Content { get; set; }
    }
}
