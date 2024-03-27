namespace ApiMailServer.Db
{
    public partial class Message
    {
        public Guid Id { get; set; }
        public Guid ProducerId { get; set; }
        public Guid ConsumerId { get; set; }
        public string? ProducerEmail { get; set; }
        public string? Content { get; set; }
        public MessageStatus Status { get; set; }
    }
}
