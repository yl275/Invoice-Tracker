using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Application.DTOs.Product;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Interfaces.Services;

namespace InvoiceSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserContext _userContext;

        public ProductService(IProductRepository productRepository, IUserContext userContext)
        {
            _productRepository = productRepository;
            _userContext = userContext;
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price
            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.ListAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price
            });
        }

        public async Task<ProductDto> AddProductAsync(CreateProductDto createProductDto)
        {
            if (!_userContext.HasUser)
                throw new UnauthorizedAccessException("User must be authenticated to add a product.");

            if (createProductDto.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            var product = new Product(_userContext.UserId!, createProductDto.Name, createProductDto.SKU, createProductDto.Price);
            await _productRepository.AddAsync(product);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price
            };
        }

        public async Task UpdateProductAsync(Guid id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.UpdateDetails(updateProductDto.Name, updateProductDto.SKU);
                product.UpdatePrice(updateProductDto.Price);
                await _productRepository.UpdateAsync(product);
            }
        }
    }
}
