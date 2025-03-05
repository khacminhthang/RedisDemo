using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisDemo.Controllers;

[ApiController]
[Route("api/redis")]
public class RedisController : ControllerBase
{
    private readonly IDatabase _database;

    public RedisController(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    // Lưu dữ liệu vào Redis
    [HttpPost("set")]
    public async Task<IActionResult> SetValue([FromQuery] string key, [FromQuery] string value)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            return BadRequest("Key và value không được để trống");

        await _database.StringSetAsync(key, value, TimeSpan.FromMinutes(10));
        return Ok($"Đã lưu {key}: {value}");
    }

    // Lấy dữ liệu từ Redis
    [HttpGet("get")]
    public async Task<IActionResult> GetValue([FromQuery] string key)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Key không được để trống");

        var value = await _database.StringGetAsync(key);
        return value.HasValue ? Ok(value.ToString()) : NotFound("Không tìm thấy giá trị");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteKey([FromQuery] string key)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Key không được để trống");

        bool deleted = await _database.KeyDeleteAsync(key);
        return deleted ? Ok($"Đã xóa key: {key}") : NotFound("Key không tồn tại");
    }

}