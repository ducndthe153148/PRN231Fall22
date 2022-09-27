using DemoOdata.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace DemoOdata.Controllers
{
    public class GadgetsOdataController : Controller
    {
        private OdataASPNetContext _context;
        public GadgetsOdataController(OdataASPNetContext context)
        {
            _context = context;
        }
        
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Gadgets.AsQueryable());
        }
    }
}
