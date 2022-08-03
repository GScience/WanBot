namespace WanBot.Plugin.EssentialPermission
{
    public class PermissionConfig
    {
        public class PermissionGroup
        {
            public string Name { get; set; } = "";

            public List<string> Permissions { get; set; } = new();
        }

        public List<PermissionGroup> GroupPermissionGroups { get; set; } = new()
        {
            new PermissionGroup()
            {
                Name = "Default",
                Permissions = new() { "Permission.User" }
            }
        };

        public string DefaultGroupGroup { get; set; } = "Default";

        /// <summary>
        /// 管理员账户列表
        /// </summary>
        public List<long> Admin { get; set; } = new();
    }
}