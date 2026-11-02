using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages;

public class ForumsModel : PageModel
{
    private readonly ForumService _forums;
    private readonly ILogger<ForumsModel> _logger;

    public List<Forum> Items { get; private set; } = new();

    public ForumsModel(
        ForumService forums,
        ILogger<ForumsModel> logger)
    {
        _forums = forums;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        Items = await _forums.GetAllAsync();

        _logger.LogInformation(
            "Loaded {Count} forums",
            Items.Count);
    }
}
