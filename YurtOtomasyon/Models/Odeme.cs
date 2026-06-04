namespace YurtOtomasyon.Models;

public class Odeme
{
    public int OdemeID { get; set; }
    public int OgrenciID { get; set; }
    public string OgrenciAdSoyad { get; set; } = "";
    public decimal Tutar { get; set; }
    public DateTime OdemeTarihi { get; set; } = DateTime.Today;
    public string? Aciklama { get; set; }
}
