using DemoAuth.Data.UnitOfWork;
using DemoAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Primitives;

namespace BUT.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]  // hard coded route name
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CategoryController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // [HttpGet("userinfo/{entityId:int}", Name = "Userinfo")]
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Paginated(
            [FromQuery] int pageSize,
            [FromQuery] string q,
            [FromQuery] int page)
        {
            var limit = pageSize == 0 ? 20 : pageSize;
            var offset = page * limit;

            try
            {
                var recordCount = (await _uow.Categories.SqlQueryAsync<int>($@"
                    SELECT COUNT(Id) FROM Categories 
                    WHERE @Term is NULL OR Name LIKE '%' || @Term || '%';",
                [
                    new SqliteParameter("Term", q),
                ]))
                .FirstOrDefault();

                var data = (await _uow.Categories.FromSqlAsync($@"
                    SELECT slow.* FROM Categories AS slow
                    INNER JOIN (
                        SELECT Id FROM Categories 
                        WHERE @Term is NULL OR Name LIKE '%' || @Term || '%'
                        LIMIT @Limit OFFSET @Offset
                    ) AS fast
                    USING (Id);",
                [
                    new SqliteParameter("Term", q),
                    new SqliteParameter("Offset", offset),
                    new SqliteParameter("Limit", limit)
                ]))
                .Select(x => new Select2Option { Id = x.Id, Text = x.Name ?? string.Empty });

                return Ok(
                    new ApiResponse
                    {
                        IsSuccess = true,
                        Result = new Select2Response { Results = data.ToList(), Pagination = new() { More = ((page + 1) * limit) < recordCount } },
                        StatusCode = System.Net.HttpStatusCode.OK
                    });
            }
            catch (Exception ex)
            {
                return new ObjectResult(
                    new ApiResponse
                    {
                        IsSuccess = false,
                        Result = new Select2Response { Results = [], Pagination = new Pagination() },
                        ErrorMessages = [ex.Message],
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
                    }
                )
                { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("odata", Name = "OData")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> OData()
        {
            try
            {
                var recordCount = (await _uow.Categories.SqlQueryAsync<int>($@"
                    SELECT COUNT(Id) FROM Categories;
                ", [])).FirstOrDefault();

                var queryString = Request.Query;
                int skip = queryString.TryGetValue("$skip", out StringValues Skip) ? Convert.ToInt32(Skip[0]) : 0;
                int top = queryString.TryGetValue("$top", out StringValues Take) ? Convert.ToInt32(Take[0]) : 1;

                var data = await _uow.Categories.FromSqlAsync($@"
                  SELECT slow.* FROM Categories AS slow
                    INNER JOIN (
                        SELECT Id FROM Categories 
                        LIMIT @Limit OFFSET @Offset
                    ) AS fast
                    USING (Id);",
                [
                    new SqliteParameter("Offset", skip),
                    new SqliteParameter("Limit", top)
                ]);

                return new { Items = data, Count = recordCount };
            }
            catch (Exception)
            {
                return new { Items = new List<Category>(), Count = 0 };
            }
        }
    }
}