using CargoFlow.Domain;
using System.ComponentModel;

namespace CargoFlow.Desktop.ViewModels
{
    public class ShipmentRow: INotifyPropertyChanged
    {
        public ShipmentRow(Shipment shipment)
        {
            this._shipment = shipment;
            shipment.PropertyChanged += Shipment_PropertyChanged;
        }

        private readonly Shipment _shipment;

        public event PropertyChangedEventHandler? PropertyChanged;
        public Shipment Shipment => _shipment;
        public int Id
        {
            get
            {
                return this._shipment.Id;
            }
        }
        public string Customer
        {
            get
            {
                return this._shipment.Customer.Name;
            }
        }
        public string Origin
        {
            get
            {
                return this._shipment.Origin.City;
            }
        }
        public string Destination
        {
            get
            {
                return this._shipment.Destination.City;
            }
        }
        public decimal Weight
        {
            get
            {
                return this._shipment.Weight;
            }
        }
        public string Status
        {
            get
            {
                return this._shipment.Status.ToString();
            }
        }

        private void Shipment_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Shipment.Status))
            {
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Status))
                );
            }
        }
    }
}
