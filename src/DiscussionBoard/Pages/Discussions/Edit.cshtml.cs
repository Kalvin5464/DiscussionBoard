using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions;

[Authorize]
public class EditModel : PageModel
{
    private readonly DiscussionThreadService _threadService;

    public EditModel(DiscussionThreadService threadService)
    {
        _threadService = threadService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int ThreadId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        DiscussionThread? thread = await _threadService.GetAsync(id);
        if (thread is null)
        {
            return NotFound();
        }

        if (thread.ApplicationUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid();
        }

        ThreadId = thread.Id;
        Input.Title = thread.Title;
        Input.Content = thread.Content;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ThreadId = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        DiscussionThread? thread = await _threadService.GetAsync(id);
        if (thread is null)
        {
            return NotFound();
        }

        if (thread.ApplicationUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid();
        }

        thread.Title = Input.Title;
        thread.Content = Input.Content;

        await _threadService.UpdateAsync(thread);

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
