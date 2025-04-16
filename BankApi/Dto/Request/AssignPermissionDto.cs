namespace BankApi.Dto.Request
{
    public class AssignPermissionDto
    {
        public int RoleId { get; set; }
        public List<int> PermissionId { get; set; } = new();
    }

}
