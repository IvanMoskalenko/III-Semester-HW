using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Attributes;

namespace MyNUnit
{
    /// <summary>
    /// Implementation of simple test framework
    /// </summary>
    public class MyNUnit
    {
        private enum TestStatus
        {
            Ignored,
            Errored,
            Failed,
            Passed
        };

        public ConcurrentQueue<TestInformation> TestsInformation { get; }

        public MyNUnit(string path)
        {
            var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            var notRepeatingFiles = new Dictionary<string, string>();
            foreach (var file in files)
            {
                if (!notRepeatingFiles.ContainsValue(Path.GetFileName(file)))
                {
                    notRepeatingFiles.Add(file, Path.GetFileName(file));
                }
            }

            var classes = notRepeatingFiles.Keys
                .AsParallel()
                .Select(Assembly.LoadFrom)
                .SelectMany(assembly => assembly.ExportedTypes)
                .Where(type => type.IsClass);
            var classesWithTests = classes
                .Where(type => type
                    .GetMethods()
                    .Any(methodInfo => methodInfo
                        .GetCustomAttributes()
                        .Any(attribute => attribute is Test)));
            TestsInformation = new ConcurrentQueue<TestInformation>();
            Parallel.ForEach(classesWithTests, StartTests);
        }

        /// <summary>
        /// Starts tests executing for one class
        /// </summary>
        /// <param name="classWithTests">Class to start tests</param>
        private void StartTests(Type classWithTests)
        {
            var methods = DivideMethodsByAttributes(classWithTests);

            TestAfterOrBeforeClass(methods.BeforeClass);

            Parallel.ForEach(methods.Tests, test => RunTest(classWithTests, test, methods));

            TestAfterOrBeforeClass(methods.AfterClass);
        }

        /// <summary>
        /// Runs one test
        /// </summary>
        /// <param name="type">Class where test is located</param>
        /// <param name="method">Method representing test</param>
        /// <param name="methods">List of class methods</param>
        private void RunTest(Type type, MethodBase method, MethodsList methods)
        {
            var property = (Test) Attribute.GetCustomAttribute(method, typeof(Test));
            if (property is {Ignore: { }})
            {
                TestsInformation.Enqueue(new TestInformation(
                    method.Name, TestStatus.Ignored.ToString(), property.Ignore, 0));
                return;
            }

            var instance = Activator.CreateInstance(type);
            try
            {
                TestAfterOrBefore(instance, methods.Before);
            }
            catch (Exception e)
            {
                TestsInformation.Enqueue(new TestInformation(
                    method.Name, TestStatus.Errored.ToString(), e.Message, 0));
                return;
            }

            var stopWatch = new Stopwatch();
            TestInformation innerTestInfo;
            try
            {
                stopWatch.Start();
                method.Invoke(instance, null);
                stopWatch.Stop();
                innerTestInfo = property is {Expected: { }}
                    ? new TestInformation(
                        method.Name, TestStatus.Failed.ToString(), null, stopWatch.ElapsedMilliseconds)
                    : new TestInformation(
                        method.Name, TestStatus.Passed.ToString(), null, stopWatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                innerTestInfo = property != null && e.InnerException != null &&
                                e.InnerException.GetType() != property.Expected
                    ? new TestInformation(
                        method.Name, TestStatus.Failed.ToString(), null, stopWatch.ElapsedMilliseconds)
                    : new TestInformation(
                        method.Name, TestStatus.Passed.ToString(), null, stopWatch.ElapsedMilliseconds);
            }

            try
            {
                TestAfterOrBefore(instance, methods.After);
                TestsInformation.Enqueue(innerTestInfo);
            }
            catch (Exception e)
            {
                TestsInformation.Enqueue(new TestInformation(
                    method.Name, TestStatus.Errored.ToString(), e.Message, 0));
            }
        }

        /// <summary>
        /// Runs methods before or after all tests
        /// </summary>
        /// <param name="afterOrBeforeClassMethods">List of methods to run</param>
        /// <exception cref="InvalidOperationException">One of methods is not static</exception>
        /// <exception cref="AggregateException">Exception occured while running one of methods</exception>
        private static void TestAfterOrBeforeClass(List<MethodInfo> afterOrBeforeClassMethods)
        {
            foreach (var method in afterOrBeforeClassMethods)
            {
                if (!method.IsStatic)
                {
                    throw new InvalidOperationException("BeforeClass or AfterClass methods were not static");
                }

                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception e)
                {
                    throw new AggregateException(e);
                }
            }
        }

        /// <summary>
        /// Runs before or after every test
        /// </summary>
        /// <param name="instance">Instance of class where test is located</param>
        /// <param name="methods">List of methods to run</param>
        private static void TestAfterOrBefore(object instance, List<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                method.Invoke(instance, null);
            }
        }

        /// <summary>
        /// Divides all methods in class by attributes
        /// </summary>
        /// <param name="classWithTests">Class where division must be executed</param>
        private static MethodsList DivideMethodsByAttributes(Type classWithTests)
        {
            var methodsList = new MethodsList();
            foreach (var method in classWithTests.GetMethods())
            {
                foreach (var attribute in Attribute.GetCustomAttributes(method))
                {
                    var attributeType = attribute.GetType();
                    if (attributeType == typeof(After))
                    {
                        methodsList.After.Add(method);
                    }
                    else if (attributeType == typeof(AfterClass))
                    {
                        methodsList.AfterClass.Add(method);
                    }
                    else if (attributeType == typeof(Before))
                    {
                        methodsList.Before.Add(method);
                    }
                    else if (attributeType == typeof(BeforeClass))
                    {
                        methodsList.BeforeClass.Add(method);
                    }
                    else if (attributeType == typeof(Test))
                    {
                        methodsList.Tests.Add(method);
                    }
                }
            }

            return methodsList;
        }
    }
}