﻿using FluentAssertions;
using NUnit.Framework;
using Tracer.Fody.Tests.Func.MockLoggers;

namespace Tracer.Fody.Tests.Func.LogTests
{
    [TestFixture]
    public class CallingLogMethods : FuncTestBase
    {
        [Test]
        public void Test_Single_LogCall_NoParameter_NoTracing()
        {
            string code = @"
                using System;
                using System.Diagnostics;
                using Tracer.Fody.Tests.Func.MockLoggers;

                namespace First
                {
                    public class MyClass
                    {
                        public static void Main()
                        {
                            MockLog.OuterNoParam();
                        }
                    }
                }
            ";

            var result = this.RunTest(code, new PrivateOnlyTraceLoggingFilter(), "First.MyClass::Main");
            result.Count.Should().Be(1);
            result.ElementAt(0).ShouldBeLogCall("First.MyClass::Main", "MockLogOuterNoParam");
        }

        [Test]
        public void Test_Single_LogCall_NoTracing()
        {
            string code = @"
                using System;
                using System.Diagnostics;
                using Tracer.Fody.Tests.Func.MockLoggers;

                namespace First
                {
                    public class MyClass
                    {
                        public static void Main()
                        {
                            MockLog.Outer(""Hello"");
                        }
                    }
                }
            ";

            var result = this.RunTest(code, new PrivateOnlyTraceLoggingFilter(), "First.MyClass::Main");
            result.Count.Should().Be(1);
            result.ElementAt(0).ShouldBeLogCall("First.MyClass::Main", "MockLogOuter", "Hello");
        }

        [Test]
        public void Test_Multiple_LogCalls_NoTracing()
        {
            string code = @"
                using System;
                using System.Diagnostics;
                using Tracer.Fody.Tests.Func.MockLoggers;

                namespace First
                {
                    public class MyClass
                    {
                        public static void Main()
                        {
                            MockLog.Outer(""Hello"");
                            int i  = 1;
                            MockLog.Outer(""String"", i);
                        }
                    }
                }
            ";

            var result = this.RunTest(code, new PrivateOnlyTraceLoggingFilter(), "First.MyClass::Main");
            result.Count.Should().Be(2);
            result.ElementAt(0).ShouldBeLogCall("First.MyClass::Main", "MockLogOuter", "Hello");
            result.ElementAt(1).ShouldBeLogCall("First.MyClass::Main", "MockLogOuter", "String", "1");
        }

        [Test]
        public void Test_Multiple_LogCalls_WithTracing()
        {
            string code = @"
                using System;
                using System.Diagnostics;
                using Tracer.Fody.Tests.Func.MockLoggers;

                namespace First
                {
                    public class MyClass
                    {
                        public static void Main()
                        {
                            MockLog.Outer(""Hello"");
                            int i  = 1;
                            MockLog.Outer(""String"", i);
                        }
                    }
                }
            ";

            var result = this.RunTest(code, new NullTraceLoggingFilter(), "First.MyClass::Main");
            result.Count.Should().Be(4);
            result.ElementAt(0).ShouldBeTraceEnterInto("First.MyClass::Main");
            result.ElementAt(1).ShouldBeLogCall("First.MyClass::Main", "MockLogOuter", "Hello");
            result.ElementAt(2).ShouldBeLogCall("First.MyClass::Main", "MockLogOuter", "String", "1");
            result.ElementAt(3).ShouldBeTraceLeaveFrom("First.MyClass::Main");
        }

    }
}
