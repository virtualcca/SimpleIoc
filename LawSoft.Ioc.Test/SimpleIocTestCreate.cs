using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LawSoft.Ioc.Test
{
    [TestClass]
    public class SimpleIocTestCreate
    {
        [TestMethod]
        [TestCategory("Create")]
        public void TestContainCreateInstanceImmediatelyByInterface()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>(true);
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Create")]
        public void TestContainCreateInstanceWhenUsingByInterface()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>(false);
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<ITestClassParent>());
            SimpleIoc.Default.GetInstance<ITestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Create")]
        public void TestContainCreateInstanceImmediatelyByCreator()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent>(() => new TestClassParent(), true);
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Create")]
        public void TestContainCreateInstanceWhenUsingByCreator()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent>(() => new TestClassParent(), false);
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<ITestClassParent>());
            SimpleIoc.Default.GetInstance<ITestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Create")]
        public void TestContainCreateCompleCtorInstanceWhenParamaterEnough()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>()
                .Register<ITestClassDerived1, TestClassDerived1>()
                .Register<TestClassComplexCtor, TestClassComplexCtor>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<TestClassComplexCtor>());
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<TestClassComplexCtor>());
            SimpleIoc.Default.GetInstance<TestClassComplexCtor>();
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<TestClassComplexCtor>());
        }

        [TestMethod]
        [TestCategory("Create")]
        public void TestContainCreateCompleCtorInstanceWhenParamaterNotenough()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>()
                .Register<TestClassComplexCtor, TestClassComplexCtor>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<TestClassComplexCtor>());
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<TestClassComplexCtor>());
            var instance = SimpleIoc.Default.GetInstance<TestClassComplexCtor>();
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<TestClassComplexCtor>());
            Assert.IsNull(instance.Parameter2);
            Assert.IsNotNull(instance.Parameter1);
        }
    }
}
