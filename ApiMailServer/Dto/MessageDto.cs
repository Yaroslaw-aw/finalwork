namespace ApiMailServer.Dto
{
    public class MessageDto
    {
        public Guid ConsumerId { get; set; }
        public string? Content { get; set; }
    }
}
