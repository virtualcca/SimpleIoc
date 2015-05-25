
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LawSoft.Ioc.Test
{
    [TestClass]
    public class SimpleIocTestContain
    {
        [TestMethod]
        [TestCategory("Contain")]
        public void TestConstainInstance()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());//is registed
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<ITestClassParent>());//not created
            SimpleIoc.Default.GetInstance<ITestClassParent>();//create instance
            Assert.IsTrue(SimpleIoc.Default.IsRegistered<ITestClassParent>());//is registed
            Assert.IsTrue(SimpleIoc.Default.ContainsCreated<ITestClassParent>());//is created

        }

        [TestMethod]
        [TestCategory("Contain")]
        public void TestConstructorTwoCtor()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<TestClassTwoCtor, TestClassTwoCtor>();
            var instance = SimpleIoc.Default.GetInstance<TestClassTwoCtor>();
            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.Ctor, 1);
        }

        [TestMethod]
        [TestCategory("Contain")]
        [ExpectedException(typeof(Exception))]
        public void TestConstructorNoPublicCtor()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<TestClassNoPublicCtor, TestClassNoPublicCtor>();
            var instance = SimpleIoc.Default.GetInstance<TestClassNoPublicCtor>();
            Assert.Fail();
        }
    }
}
