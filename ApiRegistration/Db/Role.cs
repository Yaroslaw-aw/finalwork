namespace ApiRegistration.Db
{
    public partial class Role
    {
        public RoleId RoleId { get; set; }
        public string? RoleName { get; set; }
        public virtual List<User>? Users { get; set; }
    }
}
