using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataClientExtensionsTests
    {
        [Test]
        public void CallService_Static_Action_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            clientMock.Setup(x => x.Execute(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p);
            clientMock.Object.CallService(() => CallService_Object.StaticAction("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/StaticAction"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallService_Instance_Action_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            clientMock.Setup(x => x.Execute(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p);
            var obj = new CallService_Object();
            clientMock.Object.CallService(() => obj.InstanceAction("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceAction"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entity"], Is.EqualTo(obj));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallService_Static_Func_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p)
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => CallService_Object.StaticFunc("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/StaticFunc"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void CallService_Instance_Func_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p)
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => new CallService_Object().InstanceFunc("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceFunc"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void CallServiceAsync_Action_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var taskSource = new TaskCompletionSource<ISDataResults>();
            taskSource.SetResult(null);
            clientMock.Setup(x => x.ExecuteAsync(It.IsAny<SDataParameters>(), CancellationToken.None))
                .Callback((SDataParameters p, CancellationToken c) => parms = p)
                .Returns(taskSource.Task);
            clientMock.Object.CallServiceAsync(() => CallService_Object.StaticAction("hello"), null, CancellationToken.None).Wait(CancellationToken.None);

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/StaticAction"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallServiceAsync_Func_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}}}});
            var taskSource = new TaskCompletionSource<ISDataResults<SDataResource>>();
            taskSource.SetResult(resultsMock.Object);
            clientMock.Setup(x => x.ExecuteAsync<SDataResource>(It.IsAny<SDataParameters>(), CancellationToken.None))
                .Callback((SDataParameters p, CancellationToken c) => parms = p)
                .Returns(taskSource.Task);
            var result = clientMock.Object.CallServiceAsync(() => CallService_Object.StaticFunc("hello"), null, CancellationToken.None).Result;

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/StaticFunc"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [SDataResource("dummy")]
        private class CallService_Object
        {
            public static void StaticAction(string arg)
            {
            }

            [SDataServiceOperation(InstancePropertyName = "entity")]
            public void InstanceAction(string arg)
            {
            }

            public static string StaticFunc(string arg)
            {
                throw new NotSupportedException();
            }

            [SDataServiceOperation(InstancePropertyName = "entity")]
            public string InstanceFunc(string arg)
            {
                throw new NotSupportedException();
            }
        }

        [Test]
        public void ResultPropertyName_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}, {"another", "dummy"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => ResultPropertyName_Object.Service());

            Assert.That(result, Is.EqualTo("world"));
        }

        private static class ResultPropertyName_Object
        {
            [SDataServiceOperation(ResultPropertyName = "value")]
            public static string Service()
            {
                throw new NotSupportedException();
            }
        }

        [Test]
        public void ComplexResult_Flat_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", new SDataResource {{"Name", "Joe"}}}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => ComplexResult_Object.Service());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Joe"));
        }

        [Test]
        public void ComplexResult_Nested_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"Name", "Joe"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => ComplexResult_Object.Service());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Joe"));
        }

        private class ComplexResult_Object
        {
            public static ComplexResult_Object Service()
            {
                throw new NotSupportedException();
            }

            public string Name { get; set; }
        }
    }
}