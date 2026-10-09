using System.Net;
using System.Net.Http.Json;
using CargoFlow.Blazor.Models;

namespace CargoFlow.Blazor.Services;

public class ShipmentApiClient(HttpClient httpClient)
{
    public async Task<ShipmentResponse[]?> GetShipmentsAsync(
        string? status,
        int? customerId,
        string? origin)
    {
        List<string> queryParams = [];

        if (!string.IsNullOrEmpty(status))
        {
            queryParams.Add($"status={Uri.EscapeDataString(status)}");
        }

        if (customerId is not null)
        {
            queryParams.Add($"customerId={customerId}");
        }

        if (!string.IsNullOrWhiteSpace(origin))
        {
            queryParams.Add($"origin={Uri.EscapeDataString(origin.Trim())}");
        }

        var queryString = queryParams.Count > 0
            ? $"?{string.Join("&", queryParams)}"
            : "";

        return await httpClient.GetFromJsonAsync<ShipmentResponse[]>(
            $"api/shipments{queryString}");
    }

    public async Task<HttpStatusCode> ChangeStatusAsync(int shipmentId, string action)
    {
        using var response = await httpClient.PostAsync(
            $"api/shipments/{shipmentId}/{action}", null);

        return response.StatusCode;
    }
    public async Task<ShipmentResponse?> CreateShipmentAsync(CreateShipmentRequest request)
    {
        using var response = await httpClient.PostAsJsonAsync("api/shipments", request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ShipmentResponse>();
    }
}
