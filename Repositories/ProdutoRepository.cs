using InventarioRestfulAPI.Models;

namespace InventarioRestfulAPI.Repositories
{
    public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext repositoryContext)
            : base(repositoryContext) { 
        }
    }
}
