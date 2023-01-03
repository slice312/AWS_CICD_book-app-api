using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

using book_app_api.Models;

namespace book_app_api.Services;

public class BooksService : IBooksService
{
    private readonly IDynamoDBContext _dynamoDbContext;

    public BooksService(IDynamoDBContext dynamoDbContext)
    {
        _dynamoDbContext = dynamoDbContext;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        List<Book> allBooks = await _dynamoDbContext.ScanAsync<Book>(new List<ScanCondition>())
            .GetRemainingAsync();
        return allBooks;
    }

    public async Task<Book> GetBookAsync(string isbn)
    {
        return await _dynamoDbContext.LoadAsync<Book>(isbn);
    }

    public async Task AddBookAsync(Book book)
    {
        await _dynamoDbContext.SaveAsync<Book>(book);
    }

    public async Task DeleteBookAsync(string isbn)
    {
        await _dynamoDbContext.DeleteAsync<Book>(isbn);
    }

    public async Task<Book> UpdateBookAsync(string isbn, Book book)
    {
        var bookInBase = await _dynamoDbContext.LoadAsync<Book>(isbn);
        if (bookInBase is null)
            throw new Exception("Not found");
        await _dynamoDbContext.SaveAsync<Book>(book);
        return book;
    }

    public async Task<List<Book>> GetBooksByTitleAsync(string title)
    {
        // Note: You can only query the tables that have a composite primary key (partition key and sort key).

        var queryFilter = new QueryFilter("title", QueryOperator.Equal, title);

        var queryOperationConfig = new QueryOperationConfig { Filter = queryFilter };

        var search = _dynamoDbContext.FromQueryAsync<Book>(queryOperationConfig);

        List<Book> searchResponse = await search.GetRemainingAsync();

        return searchResponse;
    }

    public async Task<Book> ToggleFavoriteAsync(string isbn)
    {
        var bookInBase = await _dynamoDbContext.LoadAsync<Book>(isbn);
        if (bookInBase is null)
            throw new Exception("Not Found");
        bookInBase.IsFavorite = !bookInBase.IsFavorite;
        await _dynamoDbContext.SaveAsync<Book>(bookInBase);
        return bookInBase;
    }
}