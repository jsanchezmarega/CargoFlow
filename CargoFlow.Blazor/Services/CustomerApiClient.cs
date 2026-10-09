using System.Net.Http.Json;
using CargoFlow.Blazor.Models;

namespace CargoFlow.Blazor.Services;

public class CustomerApiClient(HttpClient httpClient)
{
    public Task<CustomerResponse[]?> GetCustomersAsync()
    {
        return httpClient.GetFromJsonAsync<CustomerResponse[]>("api/customers");
    }

    public async Task<CustomerResponse?> CreateCustomerAsync(CreateCustomerRequest customer)
    {
        using var response = await httpClient.PostAsJsonAsync("api/customers", customer);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerResponse>();
    }
}
