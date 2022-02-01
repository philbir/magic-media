namespace MagicMedia;

public class SearchUserRequest
{
    public int PageNr { get; set; }

    public int PageSize { get; set; }

    public string? SearchText { get; set; }
}
