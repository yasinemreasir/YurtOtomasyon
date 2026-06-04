using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using YurtOtomasyon.Data;
using YurtOtomasyon.Models;

namespace YurtOtomasyon.Controllers;

public class IzinlerController : Controller
{
    private readonly Db _db;
    public IzinlerController(Db db) => _db = db;

    public IActionResult Index()
    {
        var izinler = _db.Query(@"SELECT i.*, o.Ad + ' ' + o.Soyad AS OgrenciAdSoyad
            FROM Izinler i INNER JOIN Ogrenciler o ON i.OgrenciID=o.OgrenciID
            ORDER BY i.OgrenciID",
            r => new Izin {
                IzinID = (int)r["IzinID"], OgrenciID = (int)r["OgrenciID"],
                OgrenciAdSoyad = r["OgrenciAdSoyad"].ToString()!,
                CikisTarihi = (DateTime)r["CikisTarihi"], DonusTarihi = (DateTime)r["DonusTarihi"],
                Aciklama = r["Aciklama"].ToString()
            });
        return View(izinler);
    }

    public IActionResult Create()
    {
        OgrencileriHazirla();
        return View(new Izin());
    }

    [HttpPost]
    public IActionResult Create(Izin izin)
    {
        _db.Execute(@"INSERT INTO Izinler (OgrenciID, CikisTarihi, DonusTarihi, Aciklama)
            VALUES (@ogrenci, @cikis, @donus, @aciklama)",
            new SqlParameter("@ogrenci", izin.OgrenciID), new SqlParameter("@cikis", izin.CikisTarihi),
            new SqlParameter("@donus", izin.DonusTarihi), new SqlParameter("@aciklama", (object?)izin.Aciklama ?? DBNull.Value));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        OgrencileriHazirla();
        var izin = _db.Query("SELECT * FROM Izinler WHERE IzinID=@id",
            r => new Izin {
                IzinID = (int)r["IzinID"], OgrenciID = (int)r["OgrenciID"],
                CikisTarihi = (DateTime)r["CikisTarihi"], DonusTarihi = (DateTime)r["DonusTarihi"],
                Aciklama = r["Aciklama"].ToString()
            },
            new SqlParameter("@id", id)).First();
        return View(izin);
    }

    [HttpPost]
    public IActionResult Edit(Izin izin)
    {
        _db.Execute(@"UPDATE Izinler SET OgrenciID=@ogrenci, CikisTarihi=@cikis,
            DonusTarihi=@donus, Aciklama=@aciklama WHERE IzinID=@id",
            new SqlParameter("@ogrenci", izin.OgrenciID), new SqlParameter("@cikis", izin.CikisTarihi),
            new SqlParameter("@donus", izin.DonusTarihi), new SqlParameter("@aciklama", (object?)izin.Aciklama ?? DBNull.Value),
            new SqlParameter("@id", izin.IzinID));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _db.Execute("DELETE FROM Izinler WHERE IzinID=@id", new SqlParameter("@id", id));
        return RedirectToAction(nameof(Index));
    }

    private void OgrencileriHazirla()
    {
        ViewBag.Ogrenciler = _db.Query("SELECT OgrenciID, Ad + ' ' + Soyad AS AdSoyad FROM Ogrenciler ORDER BY OgrenciID",
            r => new SelectListItem(r["AdSoyad"].ToString(), r["OgrenciID"].ToString()));
    }
}
