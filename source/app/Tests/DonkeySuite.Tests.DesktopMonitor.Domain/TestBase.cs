using System;
using DonkeySuite.DesktopMonitor.Domain;
using Ninject;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain
{
    [TestFixture]
    public class TestBase
    {
        protected IKernel TestKernel { get; set; }

        [SetUp]
        protected virtual void SetUp()
        {
            TestKernel = new StandardKernel();
            DependencyManager.Kernel = TestKernel;
        }

        [TearDown]
        protected virtual void TearDown()
        {
            TestKernel.Dispose();
            GC.Collect();
        }
    }
}