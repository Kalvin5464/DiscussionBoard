using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions;

[Authorize]
public class CreateModel : PageModel
{
    private readonly DiscussionThreadService _threadService;
    private readonly ForumService _forumService;

    public CreateModel(DiscussionThreadService threadService, ForumService forumService)
    {
        _threadService = threadService;
        _forumService = forumService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Forum? Forum { get; set; }

    public async Task<IActionResult> OnGetAsync(int forumId)
    {
        Forum = await _forumService.GetAsync(forumId);
        if (Forum is null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int forumId)
    {
        Forum = await _forumService.GetAsync(forumId);
        if (Forum is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Challenge();
        }

        var thread = new DiscussionThread
        {
            ForumId = forumId,
            Title = Input.Title,
            Content = Input.Content,
            ApplicationUserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _threadService.AddAsync(thread);

        return RedirectToPage("/Discussions/Details", new { threadId = thread.Id });
    }

    public class InputModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;
    }
}
