using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LawSoft.Ioc.Test
{
    [TestClass]
    public class SimpleIocTestRegistered
    {
        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassIsNoRegister()
        {
            SimpleIoc.Default.Reset();
            var result = SimpleIoc.Default.IsRegistered<ITestClassParent>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassIsRegister()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            var result = SimpleIoc.Default.IsRegistered<ITestClassParent>();
            SimpleIoc.Default.GetInstance<ITestClassParent>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassRegisterWithInstance()
        {
            SimpleIoc.Default.Reset();
            var testInstance = new TestClassParent();
            SimpleIoc.Default.Register(testInstance);
            SimpleIoc.Default.GetInstance<TestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<TestClassParent>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestClassRegisterWithInstanceUsingNull()
        {
            SimpleIoc.Default.Reset();
            TestClassParent testInstance = null;
            SimpleIoc.Default.Register<ITestClassParent>(testInstance);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestClassRegisterWithInstanceTwiceOrMore()
        {
            SimpleIoc.Default.Reset();
            ITestClassParent testInstance = new TestClassParent();
            SimpleIoc.Default.Register(testInstance);
            SimpleIoc.Default.Register(testInstance);
            Assert.Fail();//not performed here

        }

        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassRegisterWithInstanceUsingInterface()
        {
            SimpleIoc.Default.Reset();
            var testInstance = new TestClassParent();
            SimpleIoc.Default.Register<ITestClassParent>(testInstance);
            SimpleIoc.Default.GetInstance<ITestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassRegisterWithFactory()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent>(() => new TestClassParent());
            SimpleIoc.Default.GetInstance<ITestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Registered")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestClassRegisterWithFactoryUsingNull()
        {
            SimpleIoc.Default.Reset();
            Interface.Creator<ITestClassParent> creator = null;
            SimpleIoc.Default.Register(creator);
            Assert.Fail();//not performed here
        }

        [TestMethod]
        [TestCategory("Registered")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestClassRegisterWithFactoryTwiceOrMore()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent>(() => new TestClassParent());
            SimpleIoc.Default.Register<ITestClassParent>(() => new TestClassParent());
            Assert.Fail();//not performed here
        }

        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassRegisterWithType()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            SimpleIoc.Default.GetInstance<ITestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());
        }

        [TestMethod]
        [TestCategory("Registered")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestClassRegisterWithTypeTwiceOrMoreUsingDifferentType()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            SimpleIoc.Default.Register<ITestClassParent, TestClassDerived1>();
            Assert.Fail();//not performed here
        }

        [TestMethod]
        [TestCategory("Registered")]
        public void TestClassRegisterWithTypeTwiceOrMoreUsingSameType()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
        }

    }
}
