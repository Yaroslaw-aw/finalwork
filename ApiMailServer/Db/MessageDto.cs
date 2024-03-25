namespace ApiMailServer.Db
{
    public class MessageDto
    {
        public Guid ProducerId { get; set; }
        public Guid ConsumerId { get; set; }
        public string? Content { get; set; }
    }
}
