using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioRestfulAPI.Models;
using InventarioRestfulAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;

namespace InventarioRestfulAPI.Controllers
{
    // o atributo [Authorize] define que somente quem estiver autenticado poderá acessar a nossa API;
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : Controller
    {
        private readonly IProdutoRepository repository;

        // injetamos o serviço do nosso repositório no construtor da API
        public ProdutosController(IProdutoRepository _context)
        {
            repository = _context;
        }

        // AllowAnonymous – Permite acesso anônimo ou seja sem autenticaçã
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<string> Get()
        {
            // retornando apenas a data do acesso atual
            return "ProdutosController :: Acessado em: " + DateTime.Now.ToLongDateString();
        }

        // Authorize(Roles=”Perfil1,Perfil2”) – Exige que o usuário esteja autenticado e que faça parte de um dos perfis definidos.
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            var produtos = await repository.GetAll();

            if (produtos == null)
            {
                return BadRequest("Produtos é null");
            }

            return Ok(produtos.ToList());
        }

        // Retornando produto pelo Id recebido
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await repository.GetById(id);

            if(produto == null)
            {
                return NotFound("O id informado não corresponde a nenhum produto cadastrado.");
            }

            return Ok(produto);
        }

        // POST api/<controller>
        // No método POST estamos retornando um código de status 201 gerado pelo método CreatedAtAction quando um produto for criado.
        [HttpPost]
        public async Task<IActionResult> PostProduto([FromBody]Produto produto)
        {
            if(produto == null)
            {
                return BadRequest("Produto é null");
            }

            await repository.Insert(produto);

            return CreatedAtAction(nameof(GetProduto), new { Id = produto.ProdutoId }, produto);
        }

        // Atualiza um registro identificado pelo id recebido
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest($"O código do produto {id} não confere.");
            }

            try
            {
                await repository.Update(id, produto);
            }
            catch
            {
                throw;
            }

            return Ok(produto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Produto>> DeleteProduto(int id)
        {
            var produto = await repository.GetById(id);

            if(produto == null)
            {
                return NotFound($"Produto de {id} não foi encontrado");
            }

            await repository.Delete(id);

            return Ok(produto);
        }
    }
}
