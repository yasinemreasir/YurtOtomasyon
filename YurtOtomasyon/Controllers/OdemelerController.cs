using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using YurtOtomasyon.Data;
using YurtOtomasyon.Models;

namespace YurtOtomasyon.Controllers;

public class OdemelerController : Controller
{
    private readonly Db _db;
    public OdemelerController(Db db) => _db = db;

    public IActionResult Index()
    {
        var odemeler = _db.Query(@"SELECT od.*, o.Ad + ' ' + o.Soyad AS OgrenciAdSoyad
            FROM Odemeler od INNER JOIN Ogrenciler o ON od.OgrenciID=o.OgrenciID ORDER BY od.OgrenciID",
            r => new Odeme {
                OdemeID = (int)r["OdemeID"], OgrenciID = (int)r["OgrenciID"],
                OgrenciAdSoyad = r["OgrenciAdSoyad"].ToString()!, Tutar = (decimal)r["Tutar"],
                OdemeTarihi = (DateTime)r["OdemeTarihi"], Aciklama = r["Aciklama"].ToString()
            });
        return View(odemeler);
    }

    public IActionResult Create()
    {
        OgrencileriHazirla();
        return View(new Odeme());
    }

    [HttpPost]
    public IActionResult Create(Odeme odeme)
    {
        _db.Procedure("sp_OdemeEkle",
            new SqlParameter("@OgrenciID", odeme.OgrenciID), new SqlParameter("@Tutar", odeme.Tutar),
            new SqlParameter("@OdemeTarihi", odeme.OdemeTarihi), new SqlParameter("@Aciklama", (object?)odeme.Aciklama ?? DBNull.Value));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        OgrencileriHazirla();
        var odeme = _db.Query("SELECT * FROM Odemeler WHERE OdemeID=@id",
            r => new Odeme {
                OdemeID = (int)r["OdemeID"], OgrenciID = (int)r["OgrenciID"],
                Tutar = (decimal)r["Tutar"], OdemeTarihi = (DateTime)r["OdemeTarihi"], Aciklama = r["Aciklama"].ToString()
            },
            new SqlParameter("@id", id)).First();
        return View(odeme);
    }

    [HttpPost]
    public IActionResult Edit(Odeme odeme)
    {
        _db.Execute("UPDATE Odemeler SET OgrenciID=@ogrenci, Tutar=@tutar, OdemeTarihi=@tarih, Aciklama=@aciklama WHERE OdemeID=@id",
            new SqlParameter("@ogrenci", odeme.OgrenciID), new SqlParameter("@tutar", odeme.Tutar),
            new SqlParameter("@tarih", odeme.OdemeTarihi), new SqlParameter("@aciklama", (object?)odeme.Aciklama ?? DBNull.Value),
            new SqlParameter("@id", odeme.OdemeID));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _db.Execute("DELETE FROM Odemeler WHERE OdemeID=@id", new SqlParameter("@id", id));
        return RedirectToAction(nameof(Index));
    }

    private void OgrencileriHazirla()
    {
        ViewBag.Ogrenciler = _db.Query("SELECT OgrenciID, Ad + ' ' + Soyad AS AdSoyad FROM Ogrenciler ORDER BY Ad",
            r => new SelectListItem(r["AdSoyad"].ToString(), r["OgrenciID"].ToString()));
    }
}
