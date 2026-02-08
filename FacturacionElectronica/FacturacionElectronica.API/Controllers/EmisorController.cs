using Microsoft.AspNetCore.Mvc;

namespace FacturacionElectronica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmisorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Emisor controller working" });
        }

        [HttpGet("{nit}")]
        public IActionResult GetByNit(string nit)
        {
            return Ok(new { nit, message = "Emisor found" });
        }

        [HttpPost]
        public IActionResult Create([FromBody] object emisor)
        {
            return CreatedAtAction(nameof(GetByNit), new { nit = "123456789" }, emisor);
        }

        [HttpPut("{nit}")]
        public IActionResult Update(string nit, [FromBody] object emisor)
        {
            return Ok(new { nit, message = "Emisor updated" });
        }
    }
}