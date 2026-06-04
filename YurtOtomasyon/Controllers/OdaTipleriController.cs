using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using YurtOtomasyon.Data;
using YurtOtomasyon.Models;

namespace YurtOtomasyon.Controllers;

public class OdaTipleriController : Controller
{
    private readonly Db _db;
    public OdaTipleriController(Db db) => _db = db;

    public IActionResult Index()
    {
        var tipler = _db.Query("SELECT * FROM OdaTipleri ORDER BY OdaTipID",
            r => new OdaTipi { OdaTipID = (int)r["OdaTipID"], TipAdi = r["TipAdi"].ToString()!, AylikUcret = (decimal)r["AylikUcret"] });
        return View(tipler);
    }

    public IActionResult Create() => View(new OdaTipi());

    [HttpPost]
    public IActionResult Create(OdaTipi tip)
    {
        _db.Execute("INSERT INTO OdaTipleri (TipAdi, AylikUcret) VALUES (@ad, @ucret)",
            new SqlParameter("@ad", tip.TipAdi), new SqlParameter("@ucret", tip.AylikUcret));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var tip = _db.Query("SELECT * FROM OdaTipleri WHERE OdaTipID=@id",
            r => new OdaTipi { OdaTipID = (int)r["OdaTipID"], TipAdi = r["TipAdi"].ToString()!, AylikUcret = (decimal)r["AylikUcret"] },
            new SqlParameter("@id", id)).First();
        return View(tip);
    }

    [HttpPost]
    public IActionResult Edit(OdaTipi tip)
    {
        _db.Execute("UPDATE OdaTipleri SET TipAdi=@ad, AylikUcret=@ucret WHERE OdaTipID=@id",
            new SqlParameter("@ad", tip.TipAdi), new SqlParameter("@ucret", tip.AylikUcret), new SqlParameter("@id", tip.OdaTipID));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _db.Execute("DELETE FROM OdaTipleri WHERE OdaTipID=@id", new SqlParameter("@id", id));
        return RedirectToAction(nameof(Index));
    }
}
