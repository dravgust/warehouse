namespace Warehouse.API.Services.Errors.Models
{
    public record HttpErrorWrapper(int StatusCode, string Error);
}
