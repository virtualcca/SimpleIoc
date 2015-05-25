using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSoft.Ioc.Test
{
    public interface ITestClassParent
    {

    }

    public class TestClassParent : ITestClassParent
    {

    }

    public interface ITestClassDerived1
    {

    }

    public class TestClassDerived1 : TestClassParent, ITestClassDerived1
    {

    }

    public class TestClassDerived2 : TestClassParent
    {

    }

    public class TestClassComplexCtor
    {
        public ITestClassParent Parameter1;
        public ITestClassDerived1 Parameter2;

        public TestClassComplexCtor(ITestClassParent classParent, ITestClassDerived1 classDerived1)
        {
            Parameter1 = classParent;
            Parameter2 = classDerived1;
        }
    }

    public class TestClassTwoCtor
    {
        public int Ctor = 0;
        public TestClassTwoCtor()
        {
            Ctor = 1;
        }

        public TestClassTwoCtor(ITestClassParent classParent)
        {
            Ctor = 2;
        }
    }

    public class TestClassNoPublicCtor
    {
        private TestClassNoPublicCtor() { }
    }

    public interface ITestClass1 { }

    public class TestClass1 : ITestClass1
    {

    }

    public interface ITestClass2 { }
    public class TestClass2 : ITestClass2
    {

    }

    public interface ITestClass3 { }
    public class TestClass3 : ITestClass3
    {

    }

}
