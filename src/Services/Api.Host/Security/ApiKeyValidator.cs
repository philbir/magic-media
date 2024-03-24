using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace MagicMedia.Api.Security;

public interface IApiKeyValidator
{
    ValueTask<string?> ValidateAsync(string apiKey);
}

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly IOptions<ApiKeyOptions> _options;

    public ApiKeyValidator(IOptions<ApiKeyOptions> options)
    {
        _options = options;
    }

    public ValueTask<string?> ValidateAsync(string apiKey)
    {
        ApiKeyEntry? key = _options.Value.Keys.FirstOrDefault(k => k.Value == apiKey);
        return new ValueTask<string?>(key?.Name);
    }
}

public class ApiKeyOptions
{
    public IEnumerable<ApiKeyEntry> Keys { get; set; }
}

public class ApiKeyEntry
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Value { get; set; }
}
