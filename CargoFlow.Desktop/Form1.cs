using CargoFlow.Desktop.ViewModels;
using CargoFlow.Domain;
using CargoFlow.Services;
using System.ComponentModel;

namespace CargoFlow.Desktop;

public partial class Form1 : Form
{
    private readonly BindingList<ShipmentRow> _shipmentRows = new();
    private readonly BindingList<CustomerRow> _customerRows = new();
    private readonly ShipmentService _shipmentService;
    private readonly CustomerService _customerService;

    public Form1(ShipmentService shipmentService, CustomerService customerService)
    {
        InitializeComponent();
        this.Load += Form1_Load;

        shipmentGridControl.DataSource = _shipmentRows;
        customerComboBox.DataSource = _customerRows;
        customerComboBox.DisplayMember = "Name";

        shipmentGridView.PopulateColumns();
        shipmentGridView.Columns[nameof(ShipmentRow.Shipment)].Visible = false;

        this._shipmentService = shipmentService;
        this._customerService = customerService;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        await FetchDataAsync();
    }

    private async Task FetchDataAsync()
    {
        var shipments = await _shipmentService.GetShipmentsAsync();
        var customers = await _customerService.GetCustomersAsync();

        foreach (var shipment in shipments)
        {
            this._shipmentRows.Add(new ShipmentRow(shipment));
        }
        foreach (var customer in customers)
        {
            this._customerRows.Add(new CustomerRow(customer));
        }
    }

    private async void CreateButton_Click(object sender, EventArgs e)
    {
        var customerRow = customerComboBox.SelectedItem as CustomerRow;

        if (customerRow is null ||
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
            customerRow.Customer,
            originTextBox.Text,
            destinationTextBox.Text,
            weight
        );

        _shipmentRows.Add(new ShipmentRow(shipment));

        originTextBox.Clear();
        destinationTextBox.Clear();
        weightTextBox.Clear();
    }

    private async void startTransitButton_Click(object sender, EventArgs e)
    {
        await ChangeShipmentStatusesAsync(ShipmentStatus.InTransit);
    }

    private async void markAsDeliveredButton_Click(object sender, EventArgs e)
    {
        await ChangeShipmentStatusesAsync(ShipmentStatus.Delivered);
    }

    private async void cancelShipmentButton_Click(object sender, EventArgs e)
    {
        await ChangeShipmentStatusesAsync(ShipmentStatus.Cancelled);
    }

    private async Task ChangeShipmentStatusesAsync(ShipmentStatus status)
    {
        var selectedRowHandles = shipmentGridView.GetSelectedRows();
        var failedRows = new List<Shipment>();

        foreach (var rowHandle in selectedRowHandles)
        {
            var shipmentRow = shipmentGridView.GetRow(rowHandle) as ShipmentRow;

            if (shipmentRow is null)
                continue;

            try
            {
                switch (status)
                {
                    case ShipmentStatus.InTransit:
                        await _shipmentService.SetInTransitAsync(shipmentRow.Shipment);
                        break;

                    case ShipmentStatus.Delivered:
                        await _shipmentService.SetDeliveredAsync(shipmentRow.Shipment);
                        break;

                    case ShipmentStatus.Cancelled:
                        await _shipmentService.SetCancelledAsync(shipmentRow.Shipment);
                        break;
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
            MessageBox.Show($"Failed transitions in shipments: {string.Join(", ", failedRows.Select(r => "#" + r.Id))}");
        }
    }

    private async void createCustomerButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(customerNameTextBox.Text))
        {
            MessageBox.Show("Please fill in the customer name.");
            return;
        }

        var customer = await _customerService.CreateCustomerAsync(
            customerNameTextBox.Text
        );
        var customerRow = new CustomerRow(customer);

        _customerRows.Add(customerRow);
        customerComboBox.SelectedItem = customerRow;
        customerNameTextBox.Clear();
    }
}
