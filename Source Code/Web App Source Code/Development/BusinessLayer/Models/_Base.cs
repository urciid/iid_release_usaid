using IID.BusinessLayer.Domain;

namespace IID.BusinessLayer.Models
{
    public abstract class Base
    {
        protected Base()
        {
        } 

        ~Base()
        {
            if (_context != null)
                _context.Dispose();
        }

        private Entity _context;
        protected Entity Context
        {
            get
            {
                if (_context == null)
                    _context = new Entity();
                return _context;
            }
        }
    }
}