using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Repositories;
using Bookify.ShareKernel.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories.Generic;

public class GenericRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _context.Set<TEntity>();
    }

    // -----------------------
    // Basic Query Methods
    // -----------------------

    public async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    // -----------------------
    // Specification Methods
    // -----------------------

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, 
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);
    
    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(IProjectionSpecification<TEntity, TResult> specification, 
        CancellationToken cancellationToken = default) =>
        (IReadOnlyList<TResult>)await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    public async Task<TEntity?> GetSingleAsync<TResult>(IProjectionSpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);

    public TEntity? ExecuteAggregate<TResult>(IAggregateSpecification<TEntity, TResult> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefault(); 
    public async Task<TEntity?> ExecuteAggregateAsync<TResult>(IAggregateSpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<int> CountAsync(ISpecification<TEntity> specification, 
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .CountAsync(cancellationToken);
    
    public async Task<bool> AnyAsync(ISpecification<TEntity> specification, 
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .AnyAsync(cancellationToken);

    // -----------------------
    // Optional Non-Spec Versions
    // -----------------------

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _dbSet.CountAsync(cancellationToken);

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(cancellationToken);

    // -----------------------
    // Mutating Methods
    // -----------------------

    public void Add(TEntity entity) => _dbSet.Add(entity);

    public void AddRange(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void Remove(TEntity entity) => _dbSet.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
}
# region Example
/*
   - Entity dùng trong ví dụ:
        public class ProductId : IEquatable<ProductId>
        {
            public Guid Value { get; private set; }
            public ProductId(Guid value) => Value = value;
            public static ProductId NewId() => new ProductId(Guid.NewGuid());
            public bool Equals(ProductId? other) => other?.Value == Value;
            public override bool Equals(object? obj) => obj is ProductId other && Equals(other);
            public override int GetHashCode() => Value.GetHashCode();
            public override string ToString() => Value.ToString();
        }

        public class Product : Entity<ProductId> // Giả sử Entity<TId> là base class của
        {
            public string Name { get; private set; }
            public string? Description { get; private set; }
            public decimal Price { get; private set; }
            public int Stock { get; private set; }
            public DateTime CreatedDate { get; private set; }

            // Constructor cho EF Core
            private Product() : base(ProductId.NewId()) { }

            public Product(ProductId id, string name, decimal price, int stock) : base(id)
            {
                Name = name;
                Price = price;
                Stock = stock;
                CreatedDate = DateTime.UtcNow;
            }

            public void UpdateDetails(string name, string? description, decimal price)
            {
                Name = name;
                Description = description;
                Price = price;
            }

            public void UpdateStock(int newStock)
            {
                Stock = newStock;
            }
        }

        // Ví dụ DTO
        public class ProductDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        public class ProductNameAndStockDto
        {
            public string Name { get; set; }
            public int Stock { get; set; }
        }

    - Một số specifications để sử dụng trong các ví dụ:
        // Lấy Product theo Id
        public class ProductByIdSpec : Specification<Product>, ISingleResultSpecification<Product>
        {
            public ProductByIdSpec(ProductId productId)
            {
                Query.Where(p => p.Id.Equals(productId));
            }
        }

        // Lấy Product theo tên (có thể trả về nhiều)
        public class ProductsByNameSpec : Specification<Product>
        {
            public ProductsByNameSpec(string name)
            {
                Query.Where(p => p.Name.Contains(name));
            }
        }

        // Lấy Product có giá lớn hơn một giá trị nhất định
        public class ProductsByPriceSpec : Specification<Product>
        {
            public ProductsByPriceSpec(decimal minPrice)
            {
                Query.Where(p => p.Price > minPrice);
            }
        }

        // Specification cho việc phân trang
        public class PagedProductsSpec : Specification<Product>
        {
            public PagedProductsSpec(int skip, int take)
            {
                Query.Skip(skip).Take(take).OrderBy(p => p.Name);
            }
        }

        // Specification cho việc projection sang ProductDto
        public class ProductDtoProjectionSpec : Specification<Product>, IProjectionSpecification<Product, ProductDto>
        {
            public ProductDtoProjectionSpec() // Lấy tất cả và project
            {
                // Không cần Query.Where() nếu muốn lấy tất cả
            }

            public ProductDtoProjectionSpec(ProductId productId) // Lấy một Product và project
            {
                Query.Where(p => p.Id.Equals(productId));
            }

            public ProductDtoProjectionSpec(string nameFilter) // Lọc và project
            {
                Query.Where(p => p.Name.Contains(nameFilter));
            }

            // Selector để chọn cột cụ thể
            public Expression<Func<Product, ProductDto>> Selector =>
                p => new ProductDto
                {
                    Id = p.Id.Value, // Giả sử ProductId.Value là Guid
                    Name = p.Name,
                    Price = p.Price
                };
        }

        // Specification cho việc projection sang ProductNameAndStockDto (chọn cột tùy ý)
        public class ProductNameAndStockProjectionSpec : Specification<Product>, IProjectionSpecification<Product, ProductNameAndStockDto>
        {
            public ProductNameAndStockProjectionSpec(decimal minPrice)
            {
                Query.Where(p => p.Price > minPrice);
            }

            public Expression<Func<Product, ProductNameAndStockDto>> Selector =>
                p => new ProductNameAndStockDto
                {
                    Name = p.Name,
                    Stock = p.Stock
                };
        }


        // Specification cho việc tính tổng giá sản phẩm
        public class TotalProductPriceSpec : Specification<Product>, IAggregateSpecification<Product, decimal>
        {
            public TotalProductPriceSpec(string? category = null) // Ví dụ có thể filter theo category nếu có
            {
                if (!string.IsNullOrEmpty(category))
                {
                    // Query.Where(p => p.Category == category); // Giả sử có thuộc tính Category
                }
            }
            public Expression<Func<IQueryable<Product>, decimal>> AggregateSelector => query => query.Sum(p => p.Price);
        }

        // Specification cho việc tìm giá sản phẩm thấp nhất
        public class MinProductPriceSpec : Specification<Product>, IAggregateSpecification<Product, decimal>
        {
            public Expression<Func<IQueryable<Product>, decimal>> AggregateSelector => query => query.Min(p => p.Price);
        }

        // Specification cho việc tìm giá sản phẩm cao nhất
        public class MaxProductPriceSpec : Specification<Product>, IAggregateSpecification<Product, decimal>
        {
            public Expression<Func<IQueryable<Product>, decimal>> AggregateSelector => query => query.Max(p => p.Price);
        }
        
    - 1. GetByIdAsync - Lấy một thực thể bằng ID của nó.
        Query:
        public record GetProductByIdQuery(ProductId ProductId) : IRequest<Product?>;
        
        Handler:
        public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
        {
            private readonly IRepository<Product, ProductId> _productRepository;

            public GetProductByIdQueryHandler(IRepository<Product, ProductId> productRepository)
            {
                _productRepository = productRepository;
            }

            public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
            {
                // Cách 1: Dùng phương thức cơ bản của repository
                // return await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

                // Cách 2 (Khuyến khích với Specification): Dùng GetSingleAsync với Specification
                // Điều này nhất quán hơn và cho phép dễ dàng thêm các điều kiện khác vào specification nếu cần.
                // Đồng thời, nó sử dụng AsNoTracking() qua SpecificationEvaluator.
                var spec = new ProductByIdSpec(request.ProductId);
                return await _productRepository.GetSingleAsync(spec, cancellationToken);
            }
        }
         
        - GetAllAsync - Lấy tất cả các thực thể. Phương thức này trong repo đã dùng AsNoTracking().
          Query:  
          public record GetAllProductsQuery : IRequest<IReadOnlyList<Product>>;
          
          Handler:
          public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyList<Product>>
         {
            private readonly IRepository<Product, ProductId> _productRepository;

            public GetAllProductsQueryHandler(IRepository<Product, ProductId> productRepository)
            {
                _productRepository = productRepository;
            }

            public async Task<IReadOnlyList<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
            {
                // Phương thức này đã AsNoTracking() trong GenericRepository
                return await _productRepository.GetAllAsync(cancellationToken);
            }
          }     
          
        -   ListAsync - Lấy danh sách thực thể theo một specification.
            Query:
            public record GetProductsByMinPriceQuery(decimal MinPrice) : IRequest<IReadOnlyList<Product>>;       
            
            Handler:
            public class GetProductsByMinPriceQueryHandler : IRequestHandler<GetProductsByMinPriceQuery, IReadOnlyList<Product>>
            {
                private readonly IRepository<Product, ProductId> _productRepository;

                public GetProductsByMinPriceQueryHandler(IRepository<Product, ProductId> productRepository)
                {
                    _productRepository = productRepository;
                }

                public async Task<IReadOnlyList<Product>> Handle(GetProductsByMinPriceQuery request, CancellationToken cancellationToken)
                {
                    var spec = new ProductsByPriceSpec(request.MinPrice);
                    // Phương thức này đã AsNoTracking() trong GenericRepository
                    return await _productRepository.ListAsync(spec, cancellationToken);
                }
            }
            
        -   GetSingleAsync - Lấy một thực thể theo một specification.
            Query:
            public record GetProductByNameQuery(string Name) : IRequest<Product?>;

            Handler:
            public class GetProductByNameQueryHandler : IRequestHandler<GetProductByNameQuery, Product?>
            {
                private readonly IRepository<Product, ProductId> _productRepository;
                public GetProductByNameQueryHandler(IRepository<Product, ProductId> productRepository)  
                {
                    _productRepository = productRepository;                         
                }    
                public async Task<Product?> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
                {
                    var spec = new ProductsByNameSpec(request.Name);
                    // Phương thức này đã AsNoTracking() trong GenericRepository
                    return await _productRepository.GetSingleAsync(spec, cancellationToken);
                }
            }
            
        - ListAsync - Chọn cột cụ thể / Projection, lấy danh sách các đối tượng đã được project (DTOs) dựa trên specification.
            Query:
            public record GetActiveProductDtosQuery(string NameFilter) : IRequest<IReadOnlyList<ProductDto>>;
            
            public record GetProductNamesAndStocksQuery(decimal MinPrice) : IRequest<IReadOnlyList<ProductNameAndStockDto>>;
            
            Handler:
            public class GetActiveProductDtosQueryHandler : IRequestHandler<GetActiveProductDtosQuery, IReadOnlyList<ProductDto>>
            {
                private readonly IRepository<Product, ProductId> _productRepository;

                public GetActiveProductDtosQueryHandler(IRepository<Product, ProductId> productRepository)
                {
                    _productRepository = productRepository;
                }

                public async Task<IReadOnlyList<ProductDto>> Handle(GetActiveProductDtosQuery request, CancellationToken cancellationToken)
                {
                    var spec = new ProductDtoProjectionSpec(request.NameFilter);
                    // Phương thức này đã AsNoTracking() và thực hiện Select trong GenericRepository
                    return await _productRepository.ListAsync(spec, cancellationToken);
                }
            }   
            
            public class GetProductNamesAndStocksQueryHandler : IRequestHandler<GetProductNamesAndStocksQuery, IReadOnlyList<ProductNameAndStockDto>>
            {
                private readonly IRepository<Product, ProductId> _productRepository;

                public GetProductNamesAndStocksQueryHandler(IRepository<Product, ProductId> productRepository)
                {
                    _productRepository = productRepository;
                }

                public async Task<IReadOnlyList<ProductNameAndStockDto>> Handle(GetProductNamesAndStocksQuery request, CancellationToken cancellationToken)
                {
                    var spec = new ProductNameAndStockProjectionSpec(request.MinPrice);
                    return await _productRepository.ListAsync(spec, cancellationToken);
                }
            }
            
        - GetSingleAsync - Lấy một đối tượng đã được project duy nhất.
            Query:
            public record GetProductDtoByIdQuery(ProductId ProductId) : IRequest<ProductDto?>;

            Handler:
            public class GetProductDtoByIdQueryHandler : IRequestHandler<GetProductDtoByIdQuery, ProductDto?>
            {
                private readonly IRepository<Product, ProductId> _productRepository;

                public GetProductDtoByIdQueryHandler(IRepository<Product, ProductId> productRepository)
                {
                    _productRepository = productRepository;
                }

                public async Task<ProductDto?> Handle(GetProductDtoByIdQuery request, CancellationToken cancellationToken)
                {
                    var spec = new ProductDtoProjectionSpec(request.ProductId);
                    return await _productRepository.GetSingleAsync(spec, cancellationToken);
                }
            }
            
        - Phân Trang (Sử dụng ListAsync với Specification có Skip và Take)
            Query:
            public record GetPagedProductsQuery(int PageNumber, int PageSize) : IRequest<IReadOnlyList<Product>>;
            
            Handler:
            public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, IReadOnlyList<Product>>
            {
                private readonly IRepository<Product, ProductId> _productRepository;

                public GetPagedProductsQueryHandler(IRepository<Product, ProductId> productRepository)
                {
                    _productRepository = productRepository;
                }

                public async Task<IReadOnlyList<Product>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
                {
                    int skip = (request.PageNumber - 1) * request.PageSize;
                    int take = request.PageSize;
                    var spec = new PagedProductsSpec(skip, take);
                    return await _productRepository.ListAsync(spec, cancellationToken);
                }
            }
            
        - SUM với ExecuteAggregate:
            Query:
            public record GetTotalPriceOfProductsQuery(string? Category = null) : IRequest<decimal>;

            Handler:
            public class GetTotalPriceOfProductsQueryHandler : IRequestHandler<GetTotalPriceOfProductsQuery, decimal>
            {
                private readonly IRepository<Product, ProductId> _productRepository;

                public GetTotalPriceOfProductsQueryHandler(IRepository<Product, ProductId> productRepository)
                {
                    _productRepository = productRepository;
                }

                public decimal Handle(GetTotalPriceOfProductsQuery request, CancellationToken cancellationToken) // ExecuteAggregate là đồng bộ
                {
                    // Lưu ý: ExecuteAggregate trong GenericRepository là đồng bộ.
                    // Nếu muốn bất đồng bộ, cần sửa lại GenericRepository.
                    // Hoặc thực hiện logic bất đồng bộ tại đây nếu AggregateSelector phức tạp.
                    // Tuy nhiên, các hàm Sum, Min, Max thường là đồng bộ khi áp dụng trên IQueryable đã build.

                    var spec = new TotalProductPriceSpec(request.Category);
                    return _productRepository.ExecuteAggregate(spec);
                }
            }
                                                
 */
#endregion
