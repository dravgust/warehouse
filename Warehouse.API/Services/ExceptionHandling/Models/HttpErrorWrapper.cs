namespace Warehouse.API.Services.ExceptionHandling.Models
{
    public record HttpErrorWrapper(int StatusCode, string Error);
}
