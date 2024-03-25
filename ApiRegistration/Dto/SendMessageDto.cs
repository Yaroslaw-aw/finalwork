namespace ApiRegistration.Dto
{
    public class SendMessageDto
    {
        public Guid? ProducerId { get; set; }
        public Guid? ConsumerId { get; set; }
        public string? Content { get; set; }
    }
}
