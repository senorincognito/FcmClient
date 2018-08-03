using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FcmSharpClient.Contracts;
using FcmSharpClient.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement

namespace FcmSharpClient.Tests
{
    [TestFixture]
    public class FcmClientTests
    {
        [TestFixture]
        public class Ctor : FcmClientTests
        {
            [Test]
            public void ThrowsArgumentExceptionWhenFcmConfigurationIsNull()
            {
                Assert.Throws<ArgumentException>(() => new FcmClient(null, new HttpClient()));
            }

            [Test]
            public void ThrowsArgumentExceptionWhenHttpClientIsNull()
            {
                Assert.Throws<ArgumentException>(() => new FcmClient(new FcmConfiguration("xxx"), null));
            }

            [Test]
            public void ThrowsArgumentExceptionWhenApiKeyIsNull()
            {
                Assert.Throws<ArgumentException>(() => new FcmClient(new FcmConfiguration(null), null));
            }
        }

        [TestFixture]
        public class Send : FcmClientTests
        {
            private FakeHttpMessageHandler _messageHandler;
            private FcmClient _sut;

            [SetUp]
            public virtual void SetUp()
            {
                _messageHandler = new FakeHttpMessageHandler();
                _sut = new FcmClient(new FcmConfiguration("MY_FCM_APIKEY"), new HttpClient(_messageHandler));
            }

            [TestFixture]
            public class DuringInputValidation : Send
            {
                [Test]
                public async Task WhenTokensIsNull_ReturnsResultWith0SuccessfulAnd0FailedSends()
                {
                    var result = await _sut.Send(null, null, null, null);
                    Assert.AreEqual(0, result.SuccessfulSends.Length);
                    Assert.AreEqual(0, result.FailedSends.Length);
                }

                [Test]
                public async Task WhenTokensIsEmptyArray_ReturnsResultWith0SuccessfulAnd0FailedSends()
                {
                    var result = await _sut.Send(null, null, null, null);
                    Assert.AreEqual(0, result.SuccessfulSends.Length);
                    Assert.AreEqual(0, result.FailedSends.Length);
                }
            }

            [TestFixture]
            public class ForValidInput : Send
            {
                private const string Title = "TITLE";
                private const string Body = "BODY";
                private readonly string[] _tokens = new[] {"TOKEN_1", "TOKEN_2"};
                private FcmResponse _fcmResponse;

                public override void SetUp()
                {
                    base.SetUp();
                    _fcmResponse = new FcmResponse {Results = new List<FcmResponseResult>()};
                }

                private void Setup200Response()
                {
                    _messageHandler.Responses.Add(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(_fcmResponse))
                    });
                }

                [TestFixture]
                public class ForOkStatusCodeResponses : ForValidInput
                {
                    [Test]
                    public async Task UsesApiKeyFromConfiguration()
                    {
                        Setup200Response();

                        await _sut.Send(_tokens, Title, Body, null);
                        Assert.AreEqual("key", _messageHandler.Requests.First().Headers.Authorization.Scheme);
                        Assert.AreEqual("=MY_FCM_APIKEY",
                            _messageHandler.Requests.First().Headers.Authorization.Parameter);
                    }

                    [Test]
                    public async Task CallsFcmUrl()
                    {
                        Setup200Response();

                        await _sut.Send(_tokens, Title, Body, null);
                        Assert.AreEqual("https://fcm.googleapis.com/fcm/send",
                            _messageHandler.Requests.First().RequestUri.AbsoluteUri);
                    }

                    [Test]
                    public async Task SendsFcmRequest()
                    {
                        Setup200Response();

                        await _sut.Send(_tokens, Title, Body, new { key = "value" });
                        
                        var fcmRequest = JsonConvert.DeserializeObject<FcmRequest>(_messageHandler.RequestBodies.First());

                        Assert.AreEqual(Title, fcmRequest.Notification.Title);
                        Assert.AreEqual(Body, fcmRequest.Notification.Body);
                        Assert.AreEqual(_tokens, fcmRequest.RegistrationIds);

                        dynamic data = fcmRequest.Data;
                        Assert.AreEqual("value", (string) data.key);
                    }

                    [Test]
                    public async Task WhenSendDoesNotFail_Returns0ForFailedSends()
                    {
                        _fcmResponse.Results = new List<FcmResponseResult>
                        {
                            new FcmResponseResult(),
                            new FcmResponseResult()
                        };
                        Setup200Response();

                        var result = await _sut.Send(_tokens, Title, Body, null);
                        Assert.AreEqual(0, result.FailedSends.Length);
                        Assert.AreEqual(2, result.SuccessfulSends.Length);
                    }

                    [Test]
                    public async Task WhenSendFails_ReturnsAppropriateCountForFailedSends()
                    {
                        _fcmResponse.Results = new List<FcmResponseResult>
                        {
                            new FcmResponseResult(),
                            new FcmResponseResult
                            {
                                Error = "ERROR"
                            }
                        };
                        Setup200Response();

                        var result = await _sut.Send(_tokens, Title, Body, null);
                        Assert.AreEqual(1, result.FailedSends.Length);
                        Assert.AreEqual("TOKEN_2", result.FailedSends.First());
                    }
                }

                [TestFixture]
                public class ForNonOkStatusCodeResponses : ForValidInput
                {
                    [Test]
                    public void ThrowsFcmSendException()
                    {
                        _messageHandler.Responses.Add(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("")
                        });
                        Assert.ThrowsAsync<FcmSendException>(async () => await _sut.Send(_tokens, Title, Body, null));
                    }
                }
            }
        }
    }
}
