namespace YurtOtomasyon.Models;

public class Ogrenci
{
    public int OgrenciID { get; set; }
    public string TC { get; set; } = "";
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public string? Telefon { get; set; }
    public int BolumID { get; set; }
    public int OdaID { get; set; }
    public string BolumAdi { get; set; } = "";
    public string OdaNo { get; set; } = "";
}
