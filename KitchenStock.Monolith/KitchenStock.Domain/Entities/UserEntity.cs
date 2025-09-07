using KitchenStock.Domain.Common;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserPlan Plan { get; set; } = UserPlan.Basic;

    public ICollection<KitchenEntity> Kitchens { get; set; } = new List<KitchenEntity>();
}
