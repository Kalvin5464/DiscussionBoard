using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services;

public class DiscussionThreadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiscussionThreadService> _logger;

    public DiscussionThreadService(ApplicationDbContext context)
        : this(context, NullLogger<DiscussionThreadService>.Instance)
    {
    }

    public DiscussionThreadService(ApplicationDbContext context, ILogger<DiscussionThreadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DiscussionThread?> GetAsync(int id)
    {
        return await _context.DiscussionThreads
            .Include(t => t.ApplicationUser)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<List<DiscussionThread>> GetForForumAsync(int forumId)
    {
        List<DiscussionThread> threads = await _context.DiscussionThreads
            .Include(t => t.ApplicationUser)
            .Where(t => t.ForumId == forumId && !t.IsDeleted)
            .ToListAsync();

        return threads
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public async Task AddAsync(DiscussionThread thread)
    {
        _context.DiscussionThreads.Add(thread);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to save new discussion thread for forum {ForumId}", thread.ForumId);
            throw;
        }
    }

    public async Task UpdateAsync(DiscussionThread thread)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update discussion thread {ThreadId}", thread.Id);
            throw;
        }
    }
}
