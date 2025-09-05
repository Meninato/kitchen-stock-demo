namespace Shared.Common.Vault.Dtos;

public class VaultResponseDto<TData, TMetadata>
{
    public TData? Data { get; set; }
    public TMetadata? Metadata { get; set; }
}

internal class VaultInternalResponseDto<T>
{
    public VaultDataSectionDto<T>? Data { get; set; }
}

internal class VaultDataSectionDto<T>
{
    public T? Data { get; set; }
    public object? Metadata { get; set; }
}
