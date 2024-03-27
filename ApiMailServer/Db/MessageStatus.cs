namespace ApiMailServer.Db
{
    [Flags]
    public enum MessageStatus
    {
        Sent = 0,
        Delivered = 1
    }
}
