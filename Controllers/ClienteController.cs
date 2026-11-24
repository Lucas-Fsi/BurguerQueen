using BurguerQueen.Exceptions;
using BurguerQueen.Models;
using BurguerQueen.Models.ViewModels;
using BurguerQueen.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BurguerQueen.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // GET: /Cliente
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return View(clientes);
        }

        // GET: /Cliente/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return NotFound();

            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
            {
                TempData["Erro"] = "Cliente não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // GET: /Cliente/Create
        public IActionResult Create()
        {
            return View(new ClienteViewModel());
        }

        // POST: /Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Aciona o construtor e as validações de domínio (ValidarCPF e ValidarTelefone)
                var cliente = new Cliente(model.Nome, model.Telefone, model.CPF);

                // Regra de negócio: não permitir CPF duplicado no banco
                var existente = await _clienteRepository.ObterPorCpfAsync(cliente.CPF);
                if (existente != null)
                    throw new DominioException("Já existe um cliente cadastrado com este CPF.");

                await _clienteRepository.AdicionarAsync(cliente);
                TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DominioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro interno ao salvar o cliente. Tente novamente.");
                return View(model);
            }
        }

        // GET: /Cliente/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();

            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
            {
                TempData["Erro"] = "Cliente não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ClienteViewModel
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Telefone = cliente.Telefone,
                CPF = cliente.CPF
            };

            return View(viewModel);
        }

        // POST: /Cliente/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var cliente = await _clienteRepository.ObterPorIdAsync(id);
                if (cliente == null)
                    throw new DominioException("Cliente não encontrado.");

                // Verifica se o novo CPF pertence a outro cliente
                var clienteComMesmoCpf = await _clienteRepository.ObterPorCpfAsync(new string(model.CPF.Where(char.IsDigit).ToArray()));
                if (clienteComMesmoCpf != null && clienteComMesmoCpf.Id != id)
                    throw new DominioException("Este CPF já está vinculado a outro cliente.");

                // Atualiza via método de domínio
                cliente.AtualizarDados(model.Nome, model.Telefone, model.CPF);

                await _clienteRepository.AtualizarAsync(cliente);
                TempData["Sucesso"] = "Cliente atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DominioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _clienteRepository.RemoverAsync(id);
                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }
            catch (Exception)
            {
                TempData["Erro"] = "Não é possível excluir este cliente pois existem pedidos vinculados a ele.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}