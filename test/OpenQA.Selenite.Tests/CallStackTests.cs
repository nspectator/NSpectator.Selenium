﻿using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenite.Attributes;

namespace OpenQA.Selenite.Tests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class Given_call_stack
    {

        [Test]
        public void When_GetCallingMethod_is_called_Then_this_method_is_returned()
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            // if we convert this to a MethodGroup, then the System.RuntimeMethodHandle.InvokeMethod is the calling method, which is not what we want
            Func<MethodBase> fn = () => CallStack.GetCallingMethod();

            fn().Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void When_GetConstructingMethod_is_called_Then_throws_exception()
        {
            Action fn = () => CallStack.GetConstructingMethod();

            fn.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void When_GetCurrentMethod_is_called_Then_current_method_is_returned()
        {
            CallStack.GetCurrentMethod().Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void When_GetFirstNonSeleniteMethod_is_called_as_a_method_group_Then_this_method_is_returned()
        {
            Func<MethodBase> fn = CallStack.GetFirstNonSeleniteMethod;

            fn().Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void
            When_GetFirstNonSeleniteMethod_is_called_from_a_generic_method_Then_generic_method_definition_is_returned()
        {
            var result = NonGenericClass.GenericMethod<int>();

            result.Item2.Should().Be(result.Item1);
        }

        [Test]
        public void
            When_GetFirstNonSeleniteMethod_is_called_from_a_method_on_a_generic_class_Then_method_definition_on_generic_class_is_returned
            ()
        {
            var result = GenericClass<int>.GetMethodBase();

            result.Item2.Should().Be(result.Item1);
        }

        [Test]
        public void When_GetFirstNonSeleniteMethod_is_called_from_lambda_Then_this_method_is_returned()
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            // if we convert this to a MethodGroup, then the System.RuntimeMethodHandle.InvokeMethod is the calling method, which is not what we want
            Func<MethodBase> fn = () => CallStack.GetFirstNonSeleniteMethod();

            fn().Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void
            When_GetFirstNonSeleniteMethod_is_called_from_method_in_type_marked_invisible_Then_this_method_is_returned()
        {
            InvisibleClass.InvisibleMethod().Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void When_GetFirstSeleniteMethod_is_called_Then_this_method_is_returned()
        {
            CallStack.GetFirstNonSeleniteMethod().Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void When_type_that_calls_GetConstructingMethod_is_created_Then_this_method_is_returned()
        {
            var obj = new CallsGetConstructingMethodInConstructor();

            obj.ConstructingMethod.Should().Be(MethodBase.GetCurrentMethod());
        }

        [Test]
        public void
            When_type_that_inherits_from_type_that_calls_GetConstructingMethod_is_created_Then_this_method_is_returned()
        {
            var obj = new InheritsFromCallsGetConstructingMethodInConstructor();

            obj.ConstructingMethod.Should().Be(MethodBase.GetCurrentMethod());
        }
    }

    // ReSharper disable InconsistentNaming

    public class CallsGetConstructingMethodInConstructor
    {

        public CallsGetConstructingMethodInConstructor()
        {
            ConstructingMethod = CallStack.GetConstructingMethod();
        }

        public MethodBase ConstructingMethod { get; private set; }
    }

    public class InheritsFromCallsGetConstructingMethodInConstructor : CallsGetConstructingMethodInConstructor
    {
    }

    [Selenite]
    public static class InvisibleClass
    {
        public static MethodBase InvisibleMethod()
        {
            return CallStack.GetFirstNonSeleniteMethod();
        }
    }

    public static class GenericClass<T>
    {
        public static Tuple<MethodBase, MethodBase> GetMethodBase()
        {
            return new Tuple<MethodBase, MethodBase>(MethodBase.GetCurrentMethod(),
                CallStack.GetFirstNonSeleniteMethod());
        }
    }

    public static class NonGenericClass
    {
        public static Tuple<MethodBase, MethodBase> GenericMethod<T>()
        {
            return new Tuple<MethodBase, MethodBase>(MethodBase.GetCurrentMethod(),
                CallStack.GetFirstNonSeleniteMethod());
        }
    }
}