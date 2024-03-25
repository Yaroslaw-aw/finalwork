using ApiRegistration.Db;

namespace ApiRegistration.Dto
{
    public class GetUsersDto
    {
        //[System.Text.Json.Serialization.JsonIgnore]
        public Guid userId { get; set; }

        public string? Email { get; set; }
        public RoleId RoleId { get; set; }
    }
}
