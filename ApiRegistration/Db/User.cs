namespace ApiRegistration.Db
{
    public partial class User
    {
        public Guid userId { get; set; }
        public string? Email { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? Salt { get; set; }
        public RoleId RoleId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Role? Role { get; set; }
    }
}
