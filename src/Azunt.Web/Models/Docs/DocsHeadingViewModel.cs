namespace Azunt.Web.Models.Docs;

public sealed class DocsHeadingViewModel
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public required int Level { get; init; }
}
