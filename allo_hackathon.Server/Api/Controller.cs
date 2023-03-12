using Microsoft.AspNetCore.Mvc;
//using BlazorWasm.Grains;
//using BlazorWasm.Models;
using System.ComponentModel.DataAnnotations;
using Orleans;

namespace Server.Silo.Api;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly IGrainFactory _factory;

    public TodoController(IGrainFactory factory) => _factory = factory;

    [HttpGet("/")]
    public ActionResult GetAsync() => Ok();
}
