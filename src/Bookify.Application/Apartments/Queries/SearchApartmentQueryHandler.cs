using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Apartments.Dtos;
using Bookify.Application.Apartments.Specifications;
using Bookify.ShareKernel.Result;
using Dapper;

namespace Bookify.Application.Apartments.Queries;

internal sealed class SearchApartmentQueryHandler(ISqlConnectionFactory connectionFactory)
    : IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
{
    public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
    {
        // private readonly ISqlConnectionFactory _connectionFactory;
        //
        // public SearchApartmentQueryHandler(ISqlConnectionFactory connectionFactory)
        // {
        //     _connectionFactory = connectionFactory;
        // }
        using var connection = connectionFactory.CreateConnection();
        var specification = new SearchApartmentsSpecification(request.StartDate, request.EndDate);
        var apartments = await connection.QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
            specification.SqlQuery,
            (apartment, address) =>
            {
                apartment.Address = address;
                return apartment;
            },
            specification.Parameters,
            //Khi thấy cột Country, Dapper hiểu: từ cột này trở đi, các field sẽ được gán vào AddressResponse thay vì ApartmentResponse.
            splitOn: "Country");
        return apartments.ToList();
    }
}

/* Giải thích:
 * cú pháp đầy đủ:
     QueryAsync<TFirst, TSecond, TResult>(
        string sql,
        Func<TFirst, TSecond, TResult> map,
        object param = null,
        IDbTransaction transaction = null,
        bool buffered = true,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null
    )
    Type    | Ý nghĩa
    TFirst  | ApartmentResponse → Kết quả đầu tiên đọc từ database sẽ map vào ApartmentResponse.
    TSecond | AddressResponse → Sau khi tới cột Country (theo splitOn), bắt đầu map vào AddressResponse.
    TResult | ApartmentResponse → Hàm map sau đó phải trả về một ApartmentResponse.
    1. Dapper sẽ đọc kết quả trả về từ database.
    2. Nó sẽ map các cột trước Country vào ApartmentResponse:
    Ví dụ: Id, Name, Description, Price, Currency...
    3. Khi gặp cột Country (do splitOn: "Country"):
    - Dapper bắt đầu map tiếp vào AddressResponse:
    - Country, State, ZipCode, City, Street...
    4. Sau khi có apartment và address, Dapper gọi hàm map:
    (apartment, address) =>
    {
        apartment.Address = address;
        return apartment;
    }
    => Gán address vào apartment.Address rồi trả lại 1 ApartmentResponse hoàn chỉnh.
 */

