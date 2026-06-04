using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using YurtOtomasyon.Data;
using YurtOtomasyon.Models;

namespace YurtOtomasyon.Controllers;

public class BolumlerController : Controller
{
    private readonly Db _db;
    public BolumlerController(Db db) => _db = db;

    public IActionResult Index()
    {
        var bolumler = _db.Query("SELECT * FROM Bolumler ORDER BY BolumID",
            r => new Bolum { BolumID = (int)r["BolumID"], BolumAdi = r["BolumAdi"].ToString()! });
        return View(bolumler);
    }

    public IActionResult Create() => View(new Bolum());

    [HttpPost]
    public IActionResult Create(Bolum bolum)
    {
        _db.Execute("INSERT INTO Bolumler (BolumAdi) VALUES (@ad)", new SqlParameter("@ad", bolum.BolumAdi));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var bolum = _db.Query("SELECT * FROM Bolumler WHERE BolumID=@id",
            r => new Bolum { BolumID = (int)r["BolumID"], BolumAdi = r["BolumAdi"].ToString()! },
            new SqlParameter("@id", id)).First();
        return View(bolum);
    }

    [HttpPost]
    public IActionResult Edit(Bolum bolum)
    {
        _db.Execute("UPDATE Bolumler SET BolumAdi=@ad WHERE BolumID=@id",
            new SqlParameter("@ad", bolum.BolumAdi), new SqlParameter("@id", bolum.BolumID));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _db.Execute("DELETE FROM Bolumler WHERE BolumID=@id", new SqlParameter("@id", id));
        return RedirectToAction(nameof(Index));
    }
}
