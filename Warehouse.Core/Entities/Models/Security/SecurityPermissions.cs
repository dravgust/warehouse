namespace Warehouse.Core.Entities.Models.Security
{
    [Flags]
    public enum SecurityPermissions
    {
        None = 0,
        View = 1,
        Add = 2,
        Edit = 4,
        Delete = 8,
        Execute = 16,
        Grant = 32
    }
}
