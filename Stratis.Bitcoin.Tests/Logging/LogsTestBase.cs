﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Stratis.Bitcoin.Logging;
using Stratis.Bitcoin.RPC;

namespace Stratis.Bitcoin.Tests.Logging
{
    public class LogsTestBase : TestBase
    {
        private Mock<ILogger> fullNodeLogger;
        private Mock<ILoggerFactory> loggerFactory;
        private Mock<ILogger> rpcLogger;

        /// <remarks>
        /// This class is not able to work concurrently because logs is a static class.
        /// The logs class needs to be refactored first before tests can run in parallel.
        /// </remarks>
        public LogsTestBase()
        {
            this.fullNodeLogger = new Mock<ILogger>();
            this.rpcLogger = new Mock<ILogger>();
            this.loggerFactory = new Mock<ILoggerFactory>();
            this.loggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>()))
               .Returns(new Mock<ILogger>().Object);
            this.loggerFactory.Setup(l => l.CreateLogger("Stratis.Bitcoin.FullNode"))
               .Returns(this.fullNodeLogger.Object)
               .Verifiable();
            this.loggerFactory.Setup(l => l.CreateLogger("Stratis.Bitcoin.RPC"))
                .Returns(this.rpcLogger.Object);
        }

        public Mock<ILoggerFactory> LoggerFactory
        {
            get
            {
                return this.loggerFactory;
            }
        }

        public Mock<ILogger> FullNodeLogger
        {
            get
            {
                return this.fullNodeLogger;
            }
        }

        public Mock<ILogger> RPCLogger
        {
            get
            {
                return this.rpcLogger;
            }
        }

        protected void AssertLog<T>(Mock<ILogger> logger, LogLevel logLevel, string exceptionMessage, string message) where T : Exception
        {
            logger.Verify(f => f.Log<Object>(logLevel,
                It.IsAny<EventId>(),
                It.Is<object>(l => ((FormattedLogValues)l)[0].Value.ToString().EndsWith(message)),
                It.Is<T>(t => t.Message.Equals(exceptionMessage)),
                It.IsAny<Func<object, Exception, string>>()));
        }

        protected void AssertLog(Mock<ILogger> logger, LogLevel logLevel, string message)
        {
            logger.Verify(f => f.Log<Object>(logLevel,
                It.IsAny<EventId>(),
                It.Is<object>(l => ((FormattedLogValues)l)[0].Value.ToString().EndsWith(message)),
                null,
                It.IsAny<Func<object, Exception, string>>()));
        }

        protected void AssertLog(Mock<ILogger<RPCMiddleware>> logger, LogLevel logLevel, string message)
        {
            logger.Verify(f => f.Log<Object>(logLevel,
                It.IsAny<EventId>(),
                It.Is<object>(l => ((FormattedLogValues)l)[0].Value.ToString().EndsWith(message)),
                null,
                It.IsAny<Func<object, Exception, string>>()));
        }
    }
}
