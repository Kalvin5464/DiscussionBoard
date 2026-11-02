using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Forums;

public class DetailsModel : PageModel
{
    private readonly ForumService _forumService;
    private readonly DiscussionThreadService _threadService;

    public DetailsModel(ForumService forumService, DiscussionThreadService threadService)
    {
        _forumService = forumService;
        _threadService = threadService;
    }

    public Forum? Forum { get; set; }
    public List<DiscussionThread> Threads { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Forum = await _forumService.GetAsync(id);
        if (Forum is null)
        {
            return NotFound();
        }

        Threads = await _threadService.GetForForumAsync(id);
        return Page();
    }
}
