using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions;

public class DetailsModel : PageModel
{
    private readonly DiscussionThreadService _threadService;
    private readonly PostService _postService;

    public DetailsModel(DiscussionThreadService threadService, PostService postService)
    {
        _threadService = threadService;
        _postService = postService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public DiscussionThread? Thread { get; set; }
    public List<PostTreeBuilder.Node> PostTree { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int threadId)
    {
        Thread = await _threadService.GetAsync(threadId);
        if (Thread is null)
        {
            return NotFound();
        }

        List<Post> posts = await _postService.GetForThreadAsync(threadId);
        PostTree = PostTreeBuilder.Build(posts);
        return Page();
    }

    public async Task<IActionResult> OnPostReplyAsync(int threadId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(threadId);
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Challenge();
        }

        var post = new Post
        {
            DiscussionThreadId = threadId,
            ParentPostId = Input.ParentPostId,
            Title = string.Empty,
            Content = Input.Content,
            ApplicationUserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _postService.AddAsync(post);

        return RedirectToPage(new { threadId });
    }

    public class InputModel
    {
        public int? ParentPostId { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;
    }
}
