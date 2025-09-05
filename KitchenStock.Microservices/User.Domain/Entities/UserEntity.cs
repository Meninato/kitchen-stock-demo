using Shared.Common.Persistence.Abstractions;
using System.ComponentModel.DataAnnotations;
using User.Domain.Enums;

namespace User.Domain.Entities;

public class UserEntity : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserPlan Plan { get; set; } = UserPlan.Basic;

    public int MaxKitchens => Plan switch
    {
        UserPlan.Basic => 1,
        UserPlan.Premium => 5,
        _ => 1
    };
}
