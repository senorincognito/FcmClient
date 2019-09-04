using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FcmSharpClient.Contracts;
using FcmSharpClient.Exceptions;
using Newtonsoft.Json;
using Xunit;

// ReSharper disable ObjectCreationAsStatement

namespace FcmSharpClient.Tests
{
    public class FcmClientTests
    {
        public class Ctor : FcmClientTests
        {
            [Fact]
            public void ThrowsArgumentExceptionWhenFcmConfigurationIsNull()
            {
                Assert.Throws<ArgumentException>(() => new FcmClient(null, new HttpClient()));
            }

            [Fact]
            public void ThrowsArgumentExceptionWhenHttpClientIsNull()
            {
                Assert.Throws<ArgumentException>(() => new FcmClient(new FcmConfiguration("xxx"), null));
            }

            [Fact]
            public void ThrowsArgumentExceptionWhenApiKeyIsNull()
            {
                Assert.Throws<ArgumentException>(() => new FcmClient(new FcmConfiguration(null), null));
            }
        }

        public class Send : FcmClientTests
        {
            private readonly FakeHttpMessageHandler _messageHandler;
            private readonly FcmClient _sut;

            public Send()
            {
                
                _messageHandler = new FakeHttpMessageHandler();
                _sut = new FcmClient(new FcmConfiguration("MY_FCM_APIKEY"), new HttpClient(_messageHandler));
            }
            
            public class DuringInputValidation : Send
            {
                [Fact]
                public async Task WhenTokensIsNull_ReturnsResultWith0SuccessfulAnd0FailedSends()
                {
                    var result = await _sut.Send(null, null, null, null);
                    Assert.Empty(result.FailedSends);
                    Assert.Empty(result.FailedSends);
                }

                [Fact]
                public async Task WhenTokensIsEmptyArray_ReturnsResultWith0SuccessfulAnd0FailedSends()
                {
                    var result = await _sut.Send(null, null, null, null);
                    Assert.Empty(result.SuccessfulSends);
                    Assert.Empty(result.FailedSends);
                }
            }

            public class ForValidInput : Send
            {
                private const string Title = "TITLE";
                private const string Body = "BODY";
                private readonly string[] _tokens = new[] {"TOKEN_1", "TOKEN_2"};
                private readonly FcmResponse _fcmResponse;

                public ForValidInput()
                {
                    _fcmResponse = new FcmResponse {Results = new List<FcmResponseResult>()};
                }
                private void Setup200Response()
                {
                    _messageHandler.Responses.Add(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(_fcmResponse))
                    });
                }
                
                public class ForOkStatusCodeResponses : ForValidInput
                {
                    [Fact]
                    public async Task UsesApiKeyFromConfiguration()
                    {
                        Setup200Response();

                        await _sut.Send(_tokens, Title, Body, null);
                        Assert.Equal("key", _messageHandler.Requests.First().Headers.Authorization.Scheme);
                        Assert.Equal("=MY_FCM_APIKEY",
                            _messageHandler.Requests.First().Headers.Authorization.Parameter);
                    }

                    [Fact]
                    public async Task CallsFcmUrl()
                    {
                        Setup200Response();

                        await _sut.Send(_tokens, Title, Body, null);
                        Assert.Equal("https://fcm.googleapis.com/fcm/send",
                            _messageHandler.Requests.First().RequestUri.AbsoluteUri);
                    }

                    [Fact]
                    public async Task SendsFcmRequest()
                    {
                        Setup200Response();

                        await _sut.Send(_tokens, Title, Body, new { key = "value" });
                        
                        var fcmRequest = JsonConvert.DeserializeObject<FcmRequest>(_messageHandler.RequestBodies.First());

                        Assert.Equal(Title, fcmRequest.Notification.Title);
                        Assert.Equal(Body, fcmRequest.Notification.Body);
                        Assert.Equal(_tokens, fcmRequest.RegistrationIds);

                        dynamic data = fcmRequest.Data;
                        Assert.Equal("value", (string) data.key);
                    }

                    [Fact]
                    public async Task WhenSendDoesNotFail_Returns0ForFailedSends()
                    {
                        _fcmResponse.Results = new List<FcmResponseResult>
                        {
                            new FcmResponseResult(),
                            new FcmResponseResult()
                        };
                        Setup200Response();

                        var result = await _sut.Send(_tokens, Title, Body, null);
                        Assert.Empty(result.FailedSends);
                        Assert.Equal(2, result.SuccessfulSends.Length);
                    }

                    [Fact]
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
                        Assert.Single(result.FailedSends);
                        Assert.Equal("TOKEN_2", result.FailedSends.First());
                    }
                }

                public class ForNonOkStatusCodeResponses : ForValidInput
                {
                    [Fact]
                    public async Task ThrowsFcmSendException()
                    {
                        _messageHandler.Responses.Add(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("")
                        });
                        await Assert.ThrowsAsync<FcmSendException>(async () => await _sut.Send(_tokens, Title, Body, null));
                    }
                }
            }
        }
    }
}
