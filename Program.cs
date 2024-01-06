using System.Reflection;
using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints(opt =>
{
    opt.Assemblies = [Assembly.GetExecutingAssembly()];
});

var app = builder.Build();
app.UseFastEndpoints(opt =>
{
    opt.Serializer.ResponseSerializer = async (resp, obj, contentType, jsonCtx, ct) =>
    {
        resp.ContentType = contentType;
        await JsonSerializer.SerializeAsync(resp.Body, new Response("ResponseSerializer"), inputType: obj.GetType(), opt.Serializer.Options, cancellationToken: ct);

    };
});
app.Run();

public record Response(string Value);
public class GoodEndpoint : EndpointWithoutRequest<Response>
{

    public override void Configure()
    {
        Get("/good");
        AllowAnonymous();
    }

    public override async Task<Response> ExecuteAsync(CancellationToken ct)
    {
        return new("Good");
    }
}

public class BadEndpoint : EndpointWithoutRequest<Results<Ok<Response>, BadRequest>>
{
    public override void Configure()
    {
        Get("/bad");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Response>, BadRequest>> ExecuteAsync(CancellationToken ct)
    {
        return TypedResults.Ok<Response>(new("Bad"));
    }
}