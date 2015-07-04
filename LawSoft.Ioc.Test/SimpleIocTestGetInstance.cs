using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LawSoft.Ioc.Test
{
    [TestClass]
    public class SimpleIocTestGetInstance
    {
        [TestInitialize]
        public void Initial()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>()
                .Register<ITestClassDerived1, TestClassDerived1>()
                .Register<TestClassDerived2, TestClassDerived2>()
                .Register<ITestClass1, TestClass1>()
                .Register<ITestClass2, TestClass2>()
                .Register<TestClassComplexCtor, TestClassComplexCtor>();

        }

        [TestMethod]
        [TestCategory("GetInstance")]
        public void TestClassGetAllInstance()
        {
            SimpleIoc.Default.Reset();
            InitialData();
            var list = SimpleIoc.Default.GetAllInstances();
            Assert.IsTrue(list.Count() == 6);
        }

        [TestMethod]
        [TestCategory("GetInstance")]
        public void TestClassGetAllInstanceOnBase()
        {
            SimpleIoc.Default.Reset();
            InitialData();
            var list = SimpleIoc.Default.GetAllInstancesOnBase<ITestClassParent>();
            Assert.IsTrue(list.Count() == 2);
        }

        [TestMethod]
        [TestCategory("GetInstance")]
        public void TestClassGetInstanceUsingString()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            var instance = SimpleIoc.Default.GetInstance("LawSoft.Ioc.Test.ITestClassParent");
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        [TestCategory("GetInstance")]
        public void TestGetInstanceWithoutRegister()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            SimpleIoc.Default.Register<ITestClassDerived1, TestClassDerived1>();
            var instance = SimpleIoc.Default.GetInstanceWithoutRegister<TestClassComplexCtor>();
            Assert.IsNotNull(instance);
            Assert.IsFalse(SimpleIoc.Default.IsRegistered<TestClassComplexCtor>());
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<TestClassComplexCtor>());
        }

        [TestMethod]
        [TestCategory("GetInstance")]
        public void TestGetInstanceWithoutRegisterUsingType()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>();
            SimpleIoc.Default.Register<ITestClassDerived1, TestClassDerived1>();
            var instance = SimpleIoc.Default.GetInstanceWithoutRegister(typeof(TestClassComplexCtor));
            Assert.IsNotNull(instance);
            Assert.IsFalse(SimpleIoc.Default.IsRegistered<TestClassComplexCtor>());
            Assert.IsFalse(SimpleIoc.Default.ContainsCreated<TestClassComplexCtor>());
        }

        void InitialData()
        {
            SimpleIoc.Default.Register<ITestClassParent, TestClassParent>()
               .Register<ITestClassDerived1, TestClassDerived1>()
               .Register<TestClassDerived2, TestClassDerived2>()
               .Register<ITestClass1, TestClass1>()
               .Register<ITestClass2, TestClass2>()
               .Register<TestClassComplexCtor, TestClassComplexCtor>();
        }
    }
}
