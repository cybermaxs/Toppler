using System;
using Toppler.Redis.Scope;
using Moq;
using Toppler.Redis;
using StackExchange.Redis;
using Xunit;

namespace Toppler.Tests.Unit.Redis
{
    public class ScopeTest
    {
        #region Batch
        [Fact]
        public void BachScopeProvider_WhenValid_ShouldExecute()
        {
            var mockOfBatch = new Mock<IBatch>();
            mockOfBatch.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfBatch.Setup(b => b.Execute());

            var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(mockOfBatch.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);


            var provider = new BatchScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
                {
                    db.PingAsync();
                });

            Assert.True(execTask.Result);
            mockOfBatch.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public void BachScopeProvider_WhenException_ShouldNotExecute()
        {
            var mockOfBatch = new Mock<IBatch>();
            mockOfBatch.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfBatch.Setup(b => b.Execute());

           var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(mockOfBatch.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Throws<Exception>();


            var provider = new BatchScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.False(execTask.Result);
            mockOfBatch.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Never);
        }

        [Fact]
        public void BachScopeProvider_WhenFailed_ShouldNotExecute()
        {
            var mockOfBatch = new Mock<IBatch>();
            mockOfBatch.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).Throws<Exception>();
            mockOfBatch.Setup(b => b.Execute());

           var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(mockOfBatch.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);


            var provider = new BatchScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.False(execTask.Result);
            mockOfBatch.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }
        #endregion

        #region Transaction

        [Fact]
        public void TransactionScopeProvider_WhenValid_ShouldExecute()
        {
            var mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

           var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);


            var provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.True(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public void TransactionScopeProvider_WhenException_ShouldNotExecute()
        {

            var mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Throws<Exception>();

            var provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.False(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Never);
        }

        [Fact]
        public void TransactionScopeProvider_WhenFailed_ShouldExecute()
        {

            var mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(false);

            var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);

            var provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.False(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public void TransactionScopeProvider_WhenFailAndException_ShouldNotExecute()
        {

            var mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).Throws<Exception>();

            var mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            var mockOfConnectionProvider = new Mock<IRedisConnection>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);

            var provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.False(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }
        #endregion
    }
}
