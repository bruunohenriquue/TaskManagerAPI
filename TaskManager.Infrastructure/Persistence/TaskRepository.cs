using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public sealed class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.TaskItems.AsTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> ListAsync(
        TaskItemStatus? status,
        DateTime? dueDate,
        CancellationToken cancellationToken = default)
    {
        var query = _db.TaskItems.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (dueDate.HasValue)
        {
            var day = dueDate.Value.Date;
            var next = day.AddDays(1);
            query = query.Where(t => t.DueDate >= day && t.DueDate < next);
        }

        return await query.OrderBy(t => t.DueDate).ThenBy(t => t.Title).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _db.TaskItems.Remove(task);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
