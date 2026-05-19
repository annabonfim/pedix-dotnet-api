using Xunit;
using Moq;
using System;
using System.Threading.Tasks;

using Atendimentos.Application.Services;
using Atendimentos.Domain.Entities;
using Atendimentos.Domain.Repositories;

namespace Atendimentos.Tests
{
    public class ClienteServiceTests
    {
        [Fact]
        public async Task CriarAsync_DeveCriarClienteComSucesso()
        {
            // ========================
            // ARRANGE
            // ========================
            var mockRepository = new Mock<IClienteRepository>();

            mockRepository
                .Setup(r => r.CriarAsync(It.IsAny<Cliente>()))
                .ReturnsAsync((Cliente cliente) => cliente);

            var service = new ClienteService(mockRepository.Object);

            var nome = "Maria";
            var telefone = "11999999999";

            // ========================
            // ACT
            // ========================
            var resultado = await service.CriarAsync(nome, telefone);

            // ========================
            // ASSERT
            // ========================
            Assert.NotNull(resultado);

            Assert.Equal(nome, resultado.Nome);

            Assert.Equal(telefone, resultado.Telefone);

            mockRepository.Verify(
                r => r.CriarAsync(It.IsAny<Cliente>()),
                Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarCliente_QuandoExistir()
        {
            // ========================
            // ARRANGE
            // ========================
            var mockRepository = new Mock<IClienteRepository>();

            var cliente = new Cliente(
                "Maria",
                "11999999999");

            mockRepository
                .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(cliente);

            var service = new ClienteService(mockRepository.Object);

            // ========================
            // ACT
            // ========================
            var resultado = await service.ObterPorIdAsync(Guid.NewGuid());

            // ========================
            // ASSERT
            // ========================
            Assert.NotNull(resultado);

            Assert.Equal("Maria", resultado.Nome);
        }

        [Fact]
        public async Task DeletarAsync_DeveChamarRepositorio()
        {
            // ========================
            // ARRANGE
            // ========================
            var mockRepository = new Mock<IClienteRepository>();

            var service = new ClienteService(mockRepository.Object);

            var id = Guid.NewGuid();

            // ========================
            // ACT
            // ========================
            await service.DeletarAsync(id);

            // ========================
            // ASSERT
            // ========================
            mockRepository.Verify(
                r => r.DeletarAsync(id),
                Times.Once);
        }
    }
}