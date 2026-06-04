using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using YurtOtomasyon.Data;
using YurtOtomasyon.Models;

namespace YurtOtomasyon.Controllers;

public class OgrencilerController : Controller
{
    private readonly Db _db;
    public OgrencilerController(Db db) => _db = db;

    public IActionResult Index()
    {
        var ogrenciler = _db.Query("SELECT * FROM vw_OgrenciDetayListesi ORDER BY OgrenciID",
            r => new Ogrenci {
                OgrenciID = (int)r["OgrenciID"], TC = r["TC"].ToString()!, Ad = r["Ad"].ToString()!,
                Soyad = r["Soyad"].ToString()!, Telefon = r["Telefon"].ToString(),
                BolumAdi = r["BolumAdi"].ToString()!, OdaNo = r["OdaNo"].ToString()!
            });
        return View(ogrenciler);
    }

    public IActionResult Create()
    {
        SecimleriHazirla();
        return View(new Ogrenci());
    }

    [HttpPost]
    public IActionResult Create(Ogrenci ogrenci)
    {
        _db.Procedure("sp_OgrenciEkle",
            new SqlParameter("@TC", ogrenci.TC), new SqlParameter("@Ad", ogrenci.Ad),
            new SqlParameter("@Soyad", ogrenci.Soyad), new SqlParameter("@Telefon", (object?)ogrenci.Telefon ?? DBNull.Value),
            new SqlParameter("@BolumID", ogrenci.BolumID), new SqlParameter("@OdaID", ogrenci.OdaID));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        SecimleriHazirla();
        var ogrenci = _db.Query("SELECT * FROM Ogrenciler WHERE OgrenciID=@id",
            r => new Ogrenci {
                OgrenciID = (int)r["OgrenciID"], TC = r["TC"].ToString()!, Ad = r["Ad"].ToString()!,
                Soyad = r["Soyad"].ToString()!, Telefon = r["Telefon"].ToString(),
                BolumID = (int)r["BolumID"], OdaID = (int)r["OdaID"]
            },
            new SqlParameter("@id", id)).First();
        return View(ogrenci);
    }

    [HttpPost]
    public IActionResult Edit(Ogrenci ogrenci)
    {
        _db.Execute(@"UPDATE Ogrenciler SET TC=@tc, Ad=@ad, Soyad=@soyad, Telefon=@telefon,
            BolumID=@bolum, OdaID=@oda WHERE OgrenciID=@id",
            new SqlParameter("@tc", ogrenci.TC), new SqlParameter("@ad", ogrenci.Ad),
            new SqlParameter("@soyad", ogrenci.Soyad), new SqlParameter("@telefon", (object?)ogrenci.Telefon ?? DBNull.Value),
            new SqlParameter("@bolum", ogrenci.BolumID), new SqlParameter("@oda", ogrenci.OdaID),
            new SqlParameter("@id", ogrenci.OgrenciID));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _db.Execute("DELETE FROM Ogrenciler WHERE OgrenciID=@id", new SqlParameter("@id", id));
        return RedirectToAction(nameof(Index));
    }

    private void SecimleriHazirla()
    {
        ViewBag.Bolumler = _db.Query("SELECT * FROM Bolumler ORDER BY BolumAdi",
            r => new SelectListItem(r["BolumAdi"].ToString(), r["BolumID"].ToString()));
        ViewBag.Odalar = _db.Query("SELECT OdaID, OdaNo FROM Odalar ORDER BY OdaNo",
            r => new SelectListItem(r["OdaNo"].ToString(), r["OdaID"].ToString()));
    }
}
