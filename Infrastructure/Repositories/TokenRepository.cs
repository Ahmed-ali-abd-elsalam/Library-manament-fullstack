//using Application.IRepository;
//using Domain.Entities;
//using Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infrastructure.Repositories
//{
//    public class TokenRepository : ITokenRepository
//    {
//        private readonly LibraryDbContext context;

//        public TokenRepository(LibraryDbContext context)
//        {
//            this.context = context;
//        }

//        public  async Task<bool> CheckValidTokenAsync(string Token)
//        {
//            return  await context.UserTokens.AnyAsync(ut => ut.token == Token && ut.ExpiresAt < DateTime.UtcNow);
//        }

//        public async Task<UserToken> createUserTokenAsync(UserToken userToken)
//        {
//            await context.UserTokens.AddAsync(userToken);
//            await context.SaveChangesAsync();
//            return userToken;
//        }

//        public async Task<bool> DeleteUserToken(string Token)
//        {
//            UserToken userToken = await context.UserTokens.FirstOrDefaultAsync(ut => ut.token == Token);
//            if (userToken == null) return false;
//            context.UserTokens.Remove(userToken);
//            await context.SaveChangesAsync();
//            return true;
//        }
//        public async Task<bool> DeleteUserToken(UserToken userToken) {
//            context.UserTokens.Remove(userToken);
//            await context.SaveChangesAsync();
//            return true;

//        }

//        public async Task<bool> DeleteUserTokens(ICollection<UserToken> userTokens)
//        {
//            context.UserTokens.RemoveRange(userTokens);
//            await context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<UserToken?> GetUserTokenAsync(string Token)
//        {
//            return await context.UserTokens.FirstOrDefaultAsync(ut => ut.token == Token);
//        }

//        public async Task<UserToken?> GetUserTokenAsync(string UserEmail,string source)
//        {
//            return await context.UserTokens.FirstOrDefaultAsync(ut => ut.userEmail == UserEmail && ut.source==source);
//        }
//    }
//}
