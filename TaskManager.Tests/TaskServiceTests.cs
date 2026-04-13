using Moq;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Dtos;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Services;
using TaskManager.Domain;
using Xunit;

namespace TaskManager.Tests;

public sealed class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_normaliza_titulo_e_chama_repositorio()
    {
        var repo = new Mock<ITaskRepository>();
        TaskItem? salvo = null;
        repo.Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((t, _) => salvo = t)
            .Returns(Task.CompletedTask);

        var sut = new TaskService(repo.Object);
        var dto = await sut.CreateAsync(new CreateTaskRequest { Title = "  Estudar EF  " });

        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Estudar EF", dto.Title);
        Assert.NotNull(salvo);
        Assert.Equal(dto.Id, salvo!.Id);
        repo.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_lanca_quando_nao_existe()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<ITaskRepository>();
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var sut = new TaskService(repo.Object);
        var ex = await Assert.ThrowsAsync<TaskNotFoundException>(() => sut.GetByIdAsync(id));
        Assert.Equal(id, ex.TaskId);
    }

    [Fact]
    public async Task ListAsync_repassa_filtros_ao_repositorio()
    {
        var repo = new Mock<ITaskRepository>();
        var lista = new List<TaskItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "A",
                Status = TaskItemStatus.Pendente
            }
        };
        repo.Setup(r => r.ListAsync(TaskItemStatus.Pendente, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        var sut = new TaskService(repo.Object);
        var result = await sut.ListAsync(TaskItemStatus.Pendente, null);

        Assert.Single(result);
        Assert.Equal("A", result[0].Title);
        repo.Verify(r => r.ListAsync(TaskItemStatus.Pendente, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_altera_campos_quando_encontrada()
    {
        var id = Guid.NewGuid();
        var existente = new TaskItem
        {
            Id = id,
            Title = "Antigo",
            Status = TaskItemStatus.Pendente
        };
        var repo = new Mock<ITaskRepository>();
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(existente);
        repo.Setup(r => r.UpdateAsync(existente, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new TaskService(repo.Object);
        var atualizado = await sut.UpdateAsync(id, new UpdateTaskRequest
        {
            Title = "Novo",
            Description = "d",
            DueDate = new DateTime(2026, 5, 1),
            Status = TaskItemStatus.Concluida
        });

        Assert.Equal("Novo", atualizado.Title);
        Assert.Equal(TaskItemStatus.Concluida, existente.Status);
        repo.Verify(r => r.UpdateAsync(existente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_lanca_quando_nao_existe()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<ITaskRepository>();
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var sut = new TaskService(repo.Object);
        await Assert.ThrowsAsync<TaskNotFoundException>(() => sut.DeleteAsync(id));
    }
}
