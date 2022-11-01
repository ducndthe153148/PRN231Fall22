using ProjectPRN231.DataAccess;
using System.Security.Claims;

namespace ProjectPRN231.Method
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
