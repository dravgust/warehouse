using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class UserEntity : EntityBase<long>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
