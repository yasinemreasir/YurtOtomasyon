using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using YurtOtomasyon.Data;
using YurtOtomasyon.Models;

namespace YurtOtomasyon.Controllers;

public class OdalarController : Controller
{
    private readonly Db _db;
    public OdalarController(Db db) => _db = db;

    public IActionResult Index()
    {
        var odalar = _db.Query("SELECT * FROM vw_OdaDolulukOzeti ORDER BY OdaNo",
            r => new Oda {
                OdaID = (int)r["OdaID"], OdaNo = r["OdaNo"].ToString()!, TipAdi = r["TipAdi"].ToString()!,
                Kapasite = (int)r["Kapasite"], OgrenciSayisi = (int)r["OgrenciSayisi"]
            });
        return View(odalar);
    }

    public IActionResult Create()
    {
        TipleriHazirla();
        return View(new Oda());
    }

    [HttpPost]
    public IActionResult Create(Oda oda)
    {
        _db.Execute("INSERT INTO Odalar (OdaNo, OdaTipID, Kapasite) VALUES (@no, @tip, @kapasite)",
            new SqlParameter("@no", oda.OdaNo), new SqlParameter("@tip", oda.OdaTipID), new SqlParameter("@kapasite", oda.Kapasite));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        TipleriHazirla();
        var oda = _db.Query("SELECT * FROM Odalar WHERE OdaID=@id",
            r => new Oda {
                OdaID = (int)r["OdaID"], OdaNo = r["OdaNo"].ToString()!, OdaTipID = (int)r["OdaTipID"],
                Kapasite = (int)r["Kapasite"], OgrenciSayisi = (int)r["OgrenciSayisi"]
            },
            new SqlParameter("@id", id)).First();
        return View(oda);
    }

    [HttpPost]
    public IActionResult Edit(Oda oda)
    {
        _db.Execute("UPDATE Odalar SET OdaNo=@no, OdaTipID=@tip, Kapasite=@kapasite WHERE OdaID=@id",
            new SqlParameter("@no", oda.OdaNo), new SqlParameter("@tip", oda.OdaTipID),
            new SqlParameter("@kapasite", oda.Kapasite), new SqlParameter("@id", oda.OdaID));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _db.Execute("DELETE FROM Odalar WHERE OdaID=@id", new SqlParameter("@id", id));
        return RedirectToAction(nameof(Index));
    }

    private void TipleriHazirla()
    {
        ViewBag.OdaTipleri = _db.Query("SELECT * FROM OdaTipleri ORDER BY TipAdi",
            r => new SelectListItem(r["TipAdi"].ToString(), r["OdaTipID"].ToString()));
    }
}
