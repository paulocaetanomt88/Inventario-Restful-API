using InventarioRestfulAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventarioRestfulAPI.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private AppDbContext context = null;

        // Aqui injetamos a instância do nosso contexto no construtor da API. Observe que não temos nenhum comando SQL,
        // nenhuma declaração de objetos ADO .NET como connection, command, dataset, datareader, etc.
        public GenericRepository(AppDbContext _context)
        {
            // O contexto não só trata a referência para todos os objetos recuperados do banco de dados,
            // mas também detém os estados da entidade e mantém as modificações feitas nas propriedades da entidade.
            this.context = _context;
        }

        // Este método retorna os dados como IEnumerable;
        public async Task<IEnumerable<T>> GetAll()
        {
            return await context.Set<T>().AsNoTracking().ToListAsync();
        }

        // Retorna um objeto do tipo T pelo seu id;
        public async Task<T> GetById(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        // Recebe o objeto T para realizar a inclusão no banco de dados;
        public async Task Insert(T obj)
        {
            await context.Set<T>().AddAsync(obj);
            await context.SaveChangesAsync();
        }

        //O método Update recebe o id e uma entidade e define o seu EntityState como Modified informando ao contexto
        //que a entidade foi alterada e usando o método Update() para atualizar a entidade
        public async Task Update(int id, T obj)
        {
            context.Set<T>().Update(obj);
            await context.SaveChangesAsync();
        }

        // Recebe o objeto T e realiza a exclusão no banco de dados;
        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
