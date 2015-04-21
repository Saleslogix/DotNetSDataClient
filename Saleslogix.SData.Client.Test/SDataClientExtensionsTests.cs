using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Test.Model;

#if !NET_2_0 && !NET_3_5
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataClientExtensionsTests
    {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
        [Test]
        public void Enumerate_Empty_Test()
        {
            var page = CreateCollection(Enumerable.Empty<Contact>(), 0);
            var client = CreateClient(null, page);
            var result = client.Enumerate<Contact>("dummy");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Enumerate_Normal_Test()
        {
            var page1 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var page2 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var client = CreateClient(null, page1, page2);
            var result = client.Enumerate<Contact>("dummy");

            Assert.That(result, Is.EquivalentTo(page1.Concat(page2)));
        }

        private static ISDataClient CreateClient<T>(ICollection<SDataParameters> parmsList, params T[] responseDataSequence)
        {
            var clientMock = new Mock<ISDataClient>();
            var responses = new Queue<ISDataResults<T>>(
                responseDataSequence.Select(
                    data =>
                        {
                            var responseMock = new Mock<ISDataResults<T>>();
                            responseMock.Setup(x => x.Content).Returns(data);
                            return responseMock.Object;
                        }));
            clientMock.Setup(x => x.Execute<T>(It.IsAny<SDataParameters>()))
                      .Returns(responses.Dequeue)
                      .Callback((SDataParameters parms) =>
                                    {
                                        if (parmsList != null)
                                        {
                                            parmsList.Add(parms);
                                        }
                                    });
            return clientMock.Object;
        }
#endif

#if !NET_2_0 && !NET_3_5
        [Test]
        public void EnumerateAsync_Empty_Test()
        {
            var page = CreateCollection(Enumerable.Empty<Contact>(), 0);
            var client = CreateClientAsync(null, page);
            var result = client.EnumerateAsync<Contact>("dummy").Result;

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void EnumerateAsync_Normal_Test()
        {
            var page1 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var page2 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var client = CreateClientAsync(null, page1, page2);
            var result = client.EnumerateAsync<Contact>("dummy").Result;

            Assert.That(result, Is.EquivalentTo(page1.Concat(page2)));
        }

        private static ISDataClient CreateClientAsync<T>(ICollection<SDataParameters> parmsList, params T[] responseDataSequence)
        {
            var clientMock = new Mock<ISDataClient>();
            var responses = new Queue<Task<ISDataResults<T>>>(
                responseDataSequence.Select(
                    data =>
                        {
                            var responseMock = new Mock<ISDataResults<T>>();
                            responseMock.Setup(x => x.Content).Returns(data);
                            var taskSource = new TaskCompletionSource<ISDataResults<T>>();
                            taskSource.SetResult(responseMock.Object);
                            return taskSource.Task;
                        }));
            clientMock.Setup(x => x.ExecuteAsync<T>(It.IsAny<SDataParameters>(), CancellationToken.None))
                      .Returns(responses.Dequeue)
                      .Callback((SDataParameters parms, CancellationToken cancel) =>
                                    {
                                        if (parmsList != null)
                                        {
                                            parmsList.Add(parms);
                                        }
                                    });
            return clientMock.Object;
        }
#endif

        private static SDataCollection<T> CreateCollection<T>(IEnumerable<T> collection, int? totalResults)
        {
            var list = new SDataCollection<T>();

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    list.Add(item);
                }
            }

            if (totalResults != null)
            {
                ((ISDataProtocolObject) list).Info = new SDataProtocolInfo {TotalResults = totalResults};
            }

            return list;
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
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
        public void CallService_Instance_Action_By_Selector_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            clientMock.Setup(x => x.Execute(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p);
            var obj = new CallService_Object {Key = "abc123"};
            clientMock.Object.CallService(() => obj.InstanceActionBySelector("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy('abc123')/$service/InstanceActionBySelector"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallService_Instance_Action_By_KeyProperty_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            clientMock.Setup(x => x.Execute(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p);
            var obj = new CallService_Object {Key = "abc123"};
            clientMock.Object.CallService(() => obj.InstanceActionByKeyProperty("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceActionByKeyProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entityKey"], Is.EqualTo("abc123"));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallService_Instance_Action_By_ObjectProperty_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            clientMock.Setup(x => x.Execute(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p);
            var obj = new CallService_Object();
            clientMock.Object.CallService(() => obj.InstanceActionByObjectProperty("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceActionByObjectProperty"));
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
        public void CallService_Instance_Func_By_Selector_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p)
                .Returns(resultsMock.Object);
            var obj = new CallService_Object {Key = "abc123"};
            var result = clientMock.Object.CallService(() => obj.InstanceFuncBySelector("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy('abc123')/$service/InstanceFuncBySelector"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void CallService_Instance_Func_By_KeyProperty_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p)
                .Returns(resultsMock.Object);
            var obj = new CallService_Object {Key = "abc123"};
            var result = clientMock.Object.CallService(() => obj.InstanceFuncByKeyProperty("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceFuncByKeyProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entityKey"], Is.EqualTo("abc123"));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void CallService_Instance_Func_By_ObjectProperty_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Callback((SDataParameters p) => parms = p)
                .Returns(resultsMock.Object);
            var obj = new CallService_Object();
            var result = clientMock.Object.CallService(() => obj.InstanceFuncByObjectProperty("hello"));

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceFuncByObjectProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entity"], Is.EqualTo(obj));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }
#endif

#if !NET_2_0 && !NET_3_5
        [Test]
        public void CallServiceAsync_Static_Action_Test()
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
        public void CallServiceAsync_Instance_Action_By_Selector_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var taskSource = new TaskCompletionSource<ISDataResults>();
            taskSource.SetResult(null);
            clientMock.Setup(x => x.ExecuteAsync(It.IsAny<SDataParameters>(), CancellationToken.None))
                .Callback((SDataParameters p, CancellationToken c) => parms = p)
                .Returns(taskSource.Task);
            var obj = new CallService_Object {Key = "abc123"};
            clientMock.Object.CallServiceAsync(() => obj.InstanceActionBySelector("hello"), null, CancellationToken.None).Wait(CancellationToken.None);

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy('abc123')/$service/InstanceActionBySelector"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallServiceAsync_Instance_Action_By_KeyProperty_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var taskSource = new TaskCompletionSource<ISDataResults>();
            taskSource.SetResult(null);
            clientMock.Setup(x => x.ExecuteAsync(It.IsAny<SDataParameters>(), CancellationToken.None))
                .Callback((SDataParameters p, CancellationToken c) => parms = p)
                .Returns(taskSource.Task);
            var obj = new CallService_Object {Key = "abc123"};
            clientMock.Object.CallServiceAsync(() => obj.InstanceActionByKeyProperty("hello"), null, CancellationToken.None).Wait(CancellationToken.None);

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceActionByKeyProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entityKey"], Is.EqualTo("abc123"));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallServiceAsync_Instance_Action_By_ObjectProperty_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            SDataParameters parms = null;
            var taskSource = new TaskCompletionSource<ISDataResults>();
            taskSource.SetResult(null);
            clientMock.Setup(x => x.ExecuteAsync(It.IsAny<SDataParameters>(), CancellationToken.None))
                .Callback((SDataParameters p, CancellationToken c) => parms = p)
                .Returns(taskSource.Task);
            var obj = new CallService_Object();
            clientMock.Object.CallServiceAsync(() => obj.InstanceActionByObjectProperty("hello"), null, CancellationToken.None).Wait(CancellationToken.None);

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceActionByObjectProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entity"], Is.EqualTo(obj));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
        }

        [Test]
        public void CallServiceAsync_Static_Func_Test()
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

        [Test]
        public void CallServiceAsync_Instance_Func_By_Selector_Test()
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
            var obj = new CallService_Object {Key = "abc123"};
            var result = clientMock.Object.CallServiceAsync(() => obj.InstanceFuncBySelector("hello"), null, CancellationToken.None).Result;

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy('abc123')/$service/InstanceFuncBySelector"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void CallServiceAsync_Instance_Func_By_KeyProperty_Test()
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
            var obj = new CallService_Object {Key = "abc123"};
            var result = clientMock.Object.CallServiceAsync(() => obj.InstanceFuncByKeyProperty("hello"), null, CancellationToken.None).Result;

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceFuncByKeyProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entityKey"], Is.EqualTo("abc123"));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void CallServiceAsync_Instance_Func_By_ObjectProperty_Test()
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
            var obj = new CallService_Object();
            var result = clientMock.Object.CallServiceAsync(() => obj.InstanceFuncByObjectProperty("hello"), null, CancellationToken.None).Result;

            Assert.That(parms, Is.Not.Null);
            Assert.That(parms.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(parms.Path, Is.EqualTo("dummy/$service/InstanceFuncByObjectProperty"));
            var resource = parms.Content as SDataResource;
            Assert.That(resource, Is.Not.Null);
            resource = resource["request"] as SDataResource;
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource["entity"], Is.EqualTo(obj));
            Assert.That(resource["arg"], Is.EqualTo("hello"));
            Assert.That(result, Is.EqualTo("world"));
        }
#endif

        [SDataPath("dummy")]
        private class CallService_Object
        {
            [SDataProtocolProperty]
            public string Key { get; set; }

            public static void StaticAction(string arg)
            {
            }

            [SDataServiceOperation]
            public void InstanceActionBySelector(string arg)
            {
            }

            [SDataServiceOperation(PassInstanceBy = InstancePassingConvention.KeyProperty, InstancePropertyName = "entityKey")]
            public void InstanceActionByKeyProperty(string arg)
            {
            }

            [SDataServiceOperation(PassInstanceBy = InstancePassingConvention.ObjectProperty, InstancePropertyName = "entity")]
            public void InstanceActionByObjectProperty(string arg)
            {
            }

            public static string StaticFunc(string arg)
            {
                throw new NotSupportedException();
            }

            public string InstanceFuncBySelector(string arg)
            {
                throw new NotSupportedException();
            }

            [SDataServiceOperation(PassInstanceBy = InstancePassingConvention.KeyProperty, InstancePropertyName = "entityKey")]
            public string InstanceFuncByKeyProperty(string arg)
            {
                throw new NotSupportedException();
            }

            [SDataServiceOperation(PassInstanceBy = InstancePassingConvention.ObjectProperty, InstancePropertyName = "entity")]
            public string InstanceFuncByObjectProperty(string arg)
            {
                throw new NotSupportedException();
            }
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        [Test]
        public void CallService_ResultPropertyName_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", "world"}, {"another", "dummy"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => CallService_ResultPropertyName_Object.Service());

            Assert.That(result, Is.EqualTo("world"));
        }

        private static class CallService_ResultPropertyName_Object
        {
            [SDataServiceOperation(ResultPropertyName = "value")]
            public static string Service()
            {
                throw new NotSupportedException();
            }
        }

        [Test]
        public void CallService_ComplexResult_Flat_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"value", new SDataResource {{"Name", "Joe"}}}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => CallService_ComplexResult_Object.Service());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Joe"));
        }

        [Test]
        public void CallService_ComplexResult_Nested_Test()
        {
            var clientMock = new Mock<ISDataClient>();
            var resultsMock = new Mock<ISDataResults<SDataResource>>();
            resultsMock.Setup(x => x.Content).Returns(new SDataResource {{"response", new SDataResource {{"Name", "Joe"}}}});
            clientMock.Setup(x => x.Execute<SDataResource>(It.IsAny<SDataParameters>()))
                .Returns(resultsMock.Object);
            var result = clientMock.Object.CallService(() => CallService_ComplexResult_Object.Service());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Joe"));
        }

        private class CallService_ComplexResult_Object
        {
            public static CallService_ComplexResult_Object Service()
            {
                throw new NotSupportedException();
            }

            public string Name { get; set; }
        }
#endif
    }
}