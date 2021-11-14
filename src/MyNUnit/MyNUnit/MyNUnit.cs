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
    public class MyNUnit
    {
        private MethodsLists _methods;

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

        private void StartTests(Type classWithTests)
        {
            DivideMethodsByAttributes(classWithTests);
            TestAfterOrBeforeClass(_methods.BeforeClass);
            
            Parallel.ForEach(_methods.Tests, test => RunTest(classWithTests, test));

            TestAfterOrBeforeClass(_methods.AfterClass);
        }

        private void RunTest(Type type, MethodBase method)
        {
            var property = (Test) Attribute.GetCustomAttribute(method, typeof(Test));
            if (property is {Ignore: { }})
            {
                TestsInformation.Enqueue(new TestInformation(method.Name, "Ignored", property.Ignore, 0));
                return;
            }

            var instance = Activator.CreateInstance(type);
            try
            {
                TestAfterOrBefore(type, _methods.Before);
            }
            catch (Exception e)
            {
                TestsInformation.Enqueue(new TestInformation(method.Name, "Failed", e.Message, 0));
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
                    ? new TestInformation(method.Name, "Failed", null, stopWatch.ElapsedMilliseconds)
                    : new TestInformation(method.Name, "Passed", null, stopWatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                innerTestInfo = property != null && e.InnerException != null &&
                                e.InnerException.GetType() != property.Expected
                    ? new TestInformation(method.Name, "Failed", null, stopWatch.ElapsedMilliseconds)
                    : new TestInformation(method.Name, "Passed", null, stopWatch.ElapsedMilliseconds);
            }

            try
            {
                TestAfterOrBefore(type, _methods.After);
                TestsInformation.Enqueue(innerTestInfo);
            }
            catch (Exception e)
            {
                TestsInformation.Enqueue(new TestInformation(method.Name, "Failed", e.Message, 0));
            }
        }
        
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
        
        private static void TestAfterOrBefore(Type type, List<MethodInfo> methods)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var method in methods)
            {
                method.Invoke(instance, null);
            }
        }    
        
        private void DivideMethodsByAttributes(Type classWithTests)
        {
            _methods = new MethodsLists();

            foreach (var method in classWithTests.GetMethods())
            {
                foreach (var attribute in Attribute.GetCustomAttributes(method))
                {
                    var attributeType = attribute.GetType();
                    if (attributeType == typeof(After))
                    {
                        _methods.After.Add(method);
                    }
                    else if (attributeType == typeof(AfterClass))
                    {
                        _methods.AfterClass.Add(method);
                    }
                    else if (attributeType == typeof(Before))
                    {
                        _methods.Before.Add(method);
                    }
                    else if (attributeType == typeof(BeforeClass))
                    {
                        _methods.BeforeClass.Add(method);
                    }
                    else if (attributeType == typeof(Test))
                    {
                        _methods.Tests.Add(method);
                    }
                }
            }
        }
    }
}