namespace YurtOtomasyon.Models;

public class Izin
{
    public int IzinID { get; set; }
    public int OgrenciID { get; set; }
    public string OgrenciAdSoyad { get; set; } = "";
    public DateTime CikisTarihi { get; set; } = DateTime.Today;
    public DateTime DonusTarihi { get; set; } = DateTime.Today;
    public string? Aciklama { get; set; }
}
