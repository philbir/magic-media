namespace MagicMedia.Bff;

public class BffOptions
{
    public string ApiUrl { get; set; }
    public bool DisableAntiForgery { get; set; }
    public string DataProtectionKeysDirectory { get; set; } = "./dp_keys";
    public bool SkipCertificateValidation { get; set; }

    public string? YarpConfigSectionName { get; set; }
}
