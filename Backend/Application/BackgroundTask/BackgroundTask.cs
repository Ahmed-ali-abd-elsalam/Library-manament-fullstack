using Application.IRepository;
using Domain.Entities;
using Microsoft.Extensions.Hosting;

namespace Application.BackgroundTask
{
    public class BackgroundTask : BackgroundService
    {
        private readonly IBorrowRecordRepository borrowRecordRepository;
        private readonly IMemberRepository memberRepository;

        public BackgroundTask(IBorrowRecordRepository borrowRecordRepository, IMemberRepository memberRepository)
        {
            this.borrowRecordRepository = borrowRecordRepository;
            this.memberRepository = memberRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.UtcNow.Hour == 0)
                {
                    Dictionary<string, Member> memberDictionary = [];
                    var BorrowRecords = await borrowRecordRepository.GetBorrowRecordsAsync(0, 100);
                    var lateRecords = BorrowRecords.Where(BR => BR.Status == borrowStatus.Approved && BR.ReturnDate <= DateOnly.FromDateTime(DateTime.UtcNow)).ToList();
                    foreach (var record in lateRecords)
                    {
                        if (memberDictionary.ContainsKey(record.MemberId))
                        {
                            memberDictionary[record.MemberId].LateReturns += 1;
                        }
                        else
                        {
                            memberDictionary[record.MemberId] = await memberRepository.GetMemberAsyncById(record.MemberId);
                            memberDictionary[record.MemberId].LateReturns += 1;
                        }
                    }


                }

            }
        }
    }
}
