using CargoFlow.Desktop.ViewModels;
using CargoFlow.Domain;
using CargoFlow.Services;
using System.ComponentModel;

namespace CargoFlow.Desktop;

public partial class Form1 : Form
{
    private readonly BindingList<ShipmentRow> _shipmentRows = new();
    private readonly ShipmentService _shipmentService;
    public Form1(ShipmentService shipmentService)
    {
        InitializeComponent();
        this.Load += Form1_Load;

        bindingSource1.DataSource = _shipmentRows;

        this._shipmentService = shipmentService;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        await FetchShipmentsAsync();
    }

    private async Task FetchShipmentsAsync()
    {
        var shipments = await _shipmentService.GetShipmentsAsync();

        foreach (var shipment in shipments)
        {
            this._shipmentRows.Add(new ShipmentRow(shipment));
        }
    }

    private async void CreateButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(customerTextBox.Text) ||
            string.IsNullOrWhiteSpace(originTextBox.Text) ||
            string.IsNullOrWhiteSpace(destinationTextBox.Text))
        {
            MessageBox.Show("Please fill in all fields.");
            return;
        }

        if (!decimal.TryParse(weightTextBox.Text, out var weight))
        {
            MessageBox.Show("Please enter a valid weight.");
            return;
        }

        var shipment = await _shipmentService.CreateShipmentAsync(
            customerTextBox.Text,
            originTextBox.Text,
            destinationTextBox.Text,
            weight
        );

        _shipmentRows.Add(new ShipmentRow(shipment));
    }

    private async void startTransitButton_Click(object sender, EventArgs e)
    {
        var rows = dataGridView1.SelectedRows;
        await ChangeShipmentStatusesAsync(rows, ShipmentStatus.InTransit);
    }

    private async void markAsDeliveredButton_Click(object sender, EventArgs e)
    {
        var rows = dataGridView1.SelectedRows;
        await ChangeShipmentStatusesAsync(rows, ShipmentStatus.Delivered);
    }

    private async void cancelShipmentButton_Click(object sender, EventArgs e)
    {
        var rows = dataGridView1.SelectedRows;
        await ChangeShipmentStatusesAsync(rows, ShipmentStatus.Cancelled);
    }

    private async Task ChangeShipmentStatusesAsync(DataGridViewSelectedRowCollection rows, ShipmentStatus status)
    {
        var failedRows = new List<Shipment>();

        foreach (DataGridViewRow row in rows)
        {
            var shipmentRow = row.DataBoundItem as ShipmentRow;

            if (shipmentRow is null)
                continue;

            try
            {
                if (status == ShipmentStatus.InTransit)
                {
                    await _shipmentService.SetInTransitAsync(shipmentRow.Shipment);
                }
                if (status == ShipmentStatus.Delivered)
                {
                    await _shipmentService.SetDeliveredAsync(shipmentRow.Shipment);
                }
                if (status == ShipmentStatus.Cancelled)
                {
                    await _shipmentService.SetCancelledAsync(shipmentRow.Shipment);
                }
            }
            catch (InvalidShipmentStateException)
            {
                failedRows.Add(shipmentRow.Shipment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save shipment: {ex.Message}");
            }

        }

        if (failedRows.Count > 0)
        {
            MessageBox.Show($"Failed transitions in shipments: {String.Join(", ", failedRows.Select(r => "#" + r.Id))}");
        }
    }
}
