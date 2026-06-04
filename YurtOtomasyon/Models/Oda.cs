namespace YurtOtomasyon.Models;

public class Oda
{
    public int OdaID { get; set; }
    public string OdaNo { get; set; } = "";
    public int OdaTipID { get; set; }
    public string TipAdi { get; set; } = "";
    public int Kapasite { get; set; }
    public int OgrenciSayisi { get; set; }
}
