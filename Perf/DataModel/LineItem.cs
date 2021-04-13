namespace DataModel
{
    public class LineItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal TotalPrice => Quantity * PricePerItem;
    }
}
