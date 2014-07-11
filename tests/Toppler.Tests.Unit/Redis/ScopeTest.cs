using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Redis.Scope;
using Moq;
using Toppler.Redis;
using StackExchange.Redis;

namespace Toppler.Tests.Unit.Redis
{
    [TestClass]
    public class ScopeTest
    {
        #region Batch
        [TestMethod]
        public void BachScopeProvider_WhenValid_ShouldExecute()
        {
            Mock<IBatch> mockOfBatch = new Mock<IBatch>();
            mockOfBatch.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfBatch.Setup(b => b.Execute());

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(mockOfBatch.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);


            BatchScopeProvider provider = new BatchScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
                {
                    db.PingAsync();
                });

            Assert.IsTrue(execTask.Result);
            mockOfBatch.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }

        [TestMethod]
        public void BachScopeProvider_WhenException_ShouldNotExecute()
        {
            Mock<IBatch> mockOfBatch = new Mock<IBatch>();
            mockOfBatch.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfBatch.Setup(b => b.Execute());

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(mockOfBatch.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Throws<Exception>();


            BatchScopeProvider provider = new BatchScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.IsFalse(execTask.Result);
            mockOfBatch.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Never);
        }

        [TestMethod]
        public void BachScopeProvider_WhenFailed_ShouldNotExecute()
        {
            Mock<IBatch> mockOfBatch = new Mock<IBatch>();
            mockOfBatch.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).Throws<Exception>();
            mockOfBatch.Setup(b => b.Execute());

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(mockOfBatch.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);


            BatchScopeProvider provider = new BatchScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.IsFalse(execTask.Result);
            mockOfBatch.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }
        #endregion

        #region Transaction

        [TestMethod]
        public void TransactionScopeProvider_WhenValid_ShouldExecute()
        {
            Mock<ITransaction> mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);


            TransactionScopeProvider provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.IsTrue(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }

        [TestMethod]
        public void TransactionScopeProvider_WhenException_ShouldNotExecute()
        {

            Mock<ITransaction> mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Throws<Exception>();

            TransactionScopeProvider provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.IsFalse(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Never);
        }

        [TestMethod]
        public void TransactionScopeProvider_WhenFailed_ShouldExecute()
        {

            Mock<ITransaction> mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(false);

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);

            TransactionScopeProvider provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.IsFalse(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }

        [TestMethod]
        public void TransactionScopeProvider_WhenFailAndException_ShouldNotExecute()
        {

            Mock<ITransaction> mockOfTransaction = new Mock<ITransaction>();
            mockOfTransaction.Setup(b => b.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(TimeSpan.Zero);
            mockOfTransaction.Setup(b => b.ExecuteAsync(It.IsAny<CommandFlags>())).Throws<Exception>();

            Mock<IDatabase> mockOfDatabase = new Mock<IDatabase>();
            mockOfDatabase.Setup(m => m.CreateTransaction(It.IsAny<object>())).Returns(mockOfTransaction.Object);

            Mock<IConnectionProvider> mockOfConnectionProvider = new Mock<IConnectionProvider>();
            mockOfConnectionProvider.Setup(p => p.GetDatabase(It.IsAny<int>())).Returns(mockOfDatabase.Object);

            TransactionScopeProvider provider = new TransactionScopeProvider(mockOfConnectionProvider.Object);
            var execTask = provider.Invoke(db =>
            {
                db.PingAsync();
            });

            Assert.IsFalse(execTask.Result);
            mockOfTransaction.Verify(b => b.PingAsync(It.IsAny<CommandFlags>()), Times.Once);
        }


        #endregion

    }
}
