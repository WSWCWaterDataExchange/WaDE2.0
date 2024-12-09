namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

public class Contact
{
    public required string Email { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string Hours { get; set; }
    public string Instructions { get; set; }
    public string Address { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string StateOrProvince { get; set; }
    public string Country { get; set; }
}