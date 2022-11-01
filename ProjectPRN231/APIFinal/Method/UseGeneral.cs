using APIFinal.DataAccess;
using System.Security.Claims;

namespace APIFinal.Method
{
    public class UseGeneral
    {
        public PRN231DBContext _context;
        public UseGeneral(PRN231DBContext _context)
        {
            this._context = _context;
        }

    }
}
