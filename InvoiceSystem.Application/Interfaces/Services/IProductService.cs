using InvoiceSystem.Application.DTOs.Product;

namespace InvoiceSystem.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<ProductDto> AddProductAsync(CreateProductDto createProductDto);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task UpdateProductAsync(Guid id, UpdateProductDto updateProductDto);
    }
}
