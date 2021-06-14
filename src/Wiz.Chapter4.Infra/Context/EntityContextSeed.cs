using System.Collections.Generic;
using System.Linq;

namespace Wiz.Chapter4.Infra.Context
{
    public class EntityContextSeed
    {
        private readonly EntityContext _context;

        public EntityContextSeed(EntityContext context)
        {
            this._context = context;
            this.SeedInitial();
        }

        public void SeedInitial()
        {
        }
    }
}
