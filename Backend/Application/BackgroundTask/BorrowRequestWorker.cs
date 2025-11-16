using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Services;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
public class BorrowRequestWorker : BackgroundService
{
    private readonly IMessageQueueConsumer _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BorrowRequestWorker(
        IMessageQueueConsumer consumer,
        IServiceScopeFactory scopeFactory)
    {
        _consumer = consumer;
        _serviceScopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Inside RabbitMq Consumer Task");
        await _consumer.ConsumeAsync<BorrowRequestMessage>("borrow_queue",
        async message =>
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var bookRepo = scope.ServiceProvider.GetRequiredService<IBookRepository>();
            var memberRepo = scope.ServiceProvider.GetRequiredService<IMemberRepository>();
            var borrowRepo = scope.ServiceProvider.GetRequiredService<IBorrowRecordRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var book = await bookRepo.GetBookAsync(message.BookId);
            var user = await memberRepo.GetMemberAsyncByEmail(message.Email);

            var borrowRecord = new BorrowRecord
            {
                BookId = message.BookId,
                MemberId = user.Id,
                Member = user,
                Book = book,
                Status = borrowStatus.Pending,
                borrowDuration = message.Duration
            };

            await borrowRepo.BorrowBookAsync(borrowRecord);
            await unitOfWork.SaveChangesAsync();

            await notifier.SendToAdminsAsync("New borrow request received.");
        });
    }
}
