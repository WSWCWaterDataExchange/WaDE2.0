using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace WesternStatesWater.WaDE.Clients.Tests;

public class FakeHttpRequestData : HttpRequestData
{
    public FakeHttpRequestData(FunctionContext functionContext, Uri url, Stream? body = null) : base(functionContext)
    {
        Url = url;
        Body = body ?? new MemoryStream();
    }

    public override Stream Body { get; } = new MemoryStream();

    public override HttpHeadersCollection Headers { get; } = new HttpHeadersCollection();

    public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = [];

    public override Uri Url { get; }

    public override IEnumerable<ClaimsIdentity> Identities { get; } = [];

    public override string Method { get; } = HttpMethod.Get.ToString();

    public override HttpResponseData CreateResponse()
    {
        return new FakeHttpResponseData(FunctionContext);
    }
}

public class FakeHttpResponseData : HttpResponseData
{
    public FakeHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
    public override Stream Body { get; set; } = new MemoryStream();
    public override HttpCookies Cookies { get; }
}