using CargoFlow.Domain;

namespace CargoFlow.Desktop.ViewModels
{
    public class CustomerRow
    {
        public CustomerRow(Customer customer)
        {
            this._customer = customer;
        }

        private readonly Customer _customer;

        public Customer Customer => _customer;
        public int Id
        {
            get
            {
                return this._customer.Id;
            }
        }
        public string Name
        {
            get
            {
                return this._customer.Name;
            }
        }
    }
}
