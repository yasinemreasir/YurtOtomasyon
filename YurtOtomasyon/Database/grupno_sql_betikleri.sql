IF DB_ID('YurtOtomasyonDB') IS NULL
    CREATE DATABASE YurtOtomasyonDB;
GO

USE YurtOtomasyonDB;
GO

DROP TRIGGER IF EXISTS trg_OgrenciEkle_OdaSayisi;
DROP TRIGGER IF EXISTS trg_OgrenciSil_OdaSayisi;
DROP PROCEDURE IF EXISTS sp_OgrenciEkle;
DROP PROCEDURE IF EXISTS sp_OdemeEkle;
DROP VIEW IF EXISTS vw_OdaDolulukOzeti;
DROP VIEW IF EXISTS vw_OgrenciDetayListesi;
DROP TABLE IF EXISTS Odemeler;
DROP TABLE IF EXISTS Ogrenciler;
DROP TABLE IF EXISTS Odalar;
DROP TABLE IF EXISTS OdaTipleri;
DROP TABLE IF EXISTS Bolumler;
GO

CREATE TABLE Bolumler (
    BolumID INT IDENTITY(1,1) PRIMARY KEY,
    BolumAdi NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE OdaTipleri (
    OdaTipID INT IDENTITY(1,1) PRIMARY KEY,
    TipAdi NVARCHAR(50) NOT NULL UNIQUE,
    AylikUcret DECIMAL(10,2) NOT NULL CHECK (AylikUcret > 0)
);

CREATE TABLE Odalar (
    OdaID INT IDENTITY(1,1) PRIMARY KEY,
    OdaNo NVARCHAR(10) NOT NULL UNIQUE,
    OdaTipID INT NOT NULL,
    Kapasite INT NOT NULL CHECK (Kapasite > 0),
    OgrenciSayisi INT NOT NULL DEFAULT 0 CHECK (OgrenciSayisi >= 0),
    CONSTRAINT FK_Odalar_OdaTipleri FOREIGN KEY (OdaTipID) REFERENCES OdaTipleri(OdaTipID)
);

CREATE TABLE Ogrenciler (
    OgrenciID INT IDENTITY(1,1) PRIMARY KEY,
    TC NVARCHAR(11) NOT NULL UNIQUE CHECK (LEN(TC) = 11),
    Ad NVARCHAR(50) NOT NULL,
    Soyad NVARCHAR(50) NOT NULL,
    Telefon NVARCHAR(20) NULL,
    BolumID INT NOT NULL,
    OdaID INT NOT NULL,
    KayitTarihi DATE NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Ogrenciler_Bolumler FOREIGN KEY (BolumID) REFERENCES Bolumler(BolumID),
    CONSTRAINT FK_Ogrenciler_Odalar FOREIGN KEY (OdaID) REFERENCES Odalar(OdaID)
);

CREATE TABLE Odemeler (
    OdemeID INT IDENTITY(1,1) PRIMARY KEY,
    OgrenciID INT NOT NULL,
    Tutar DECIMAL(10,2) NOT NULL CHECK (Tutar > 0),
    OdemeTarihi DATE NOT NULL DEFAULT GETDATE(),
    Aciklama NVARCHAR(150) NULL,
    CONSTRAINT FK_Odemeler_Ogrenciler FOREIGN KEY (OgrenciID) REFERENCES Ogrenciler(OgrenciID) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Ogrenciler_TC ON Ogrenciler(TC);
CREATE INDEX IX_Odemeler_OdemeTarihi ON Odemeler(OdemeTarihi);
GO

CREATE TRIGGER trg_OgrenciEkle_OdaSayisi
ON Ogrenciler
AFTER INSERT
AS
BEGIN
    UPDATE Odalar
    SET OgrenciSayisi = OgrenciSayisi + x.Adet
    FROM Odalar o
    INNER JOIN (SELECT OdaID, COUNT(*) AS Adet FROM inserted GROUP BY OdaID) x ON o.OdaID = x.OdaID;
END;
GO

CREATE TRIGGER trg_OgrenciSil_OdaSayisi
ON Ogrenciler
AFTER DELETE
AS
BEGIN
    UPDATE Odalar
    SET OgrenciSayisi = OgrenciSayisi - x.Adet
    FROM Odalar o
    INNER JOIN (SELECT OdaID, COUNT(*) AS Adet FROM deleted GROUP BY OdaID) x ON o.OdaID = x.OdaID;
END;
GO

CREATE VIEW vw_OgrenciDetayListesi AS
SELECT
    o.OgrenciID,
    o.TC,
    o.Ad,
    o.Soyad,
    o.Telefon,
    b.BolumAdi,
    od.OdaNo,
    ot.TipAdi AS OdaTipi
FROM Ogrenciler o
INNER JOIN Bolumler b ON o.BolumID = b.BolumID
INNER JOIN Odalar od ON o.OdaID = od.OdaID
INNER JOIN OdaTipleri ot ON od.OdaTipID = ot.OdaTipID;
GO

CREATE VIEW vw_OdaDolulukOzeti AS
SELECT
    od.OdaID,
    od.OdaNo,
    ot.TipAdi,
    od.Kapasite,
    od.OgrenciSayisi,
    (od.Kapasite - od.OgrenciSayisi) AS BosYatakSayisi
FROM Odalar od
INNER JOIN OdaTipleri ot ON od.OdaTipID = ot.OdaTipID;
GO

CREATE PROCEDURE sp_OgrenciEkle
    @TC NVARCHAR(11),
    @Ad NVARCHAR(50),
    @Soyad NVARCHAR(50),
    @Telefon NVARCHAR(20),
    @BolumID INT,
    @OdaID INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Ogrenciler WHERE TC = @TC)
    BEGIN
        RAISERROR('Bu TC ile kayitli ogrenci var.', 16, 1);
        RETURN;
    END;

    INSERT INTO Ogrenciler (TC, Ad, Soyad, Telefon, BolumID, OdaID)
    VALUES (@TC, @Ad, @Soyad, @Telefon, @BolumID, @OdaID);
END;
GO

CREATE PROCEDURE sp_OdemeEkle
    @OgrenciID INT,
    @Tutar DECIMAL(10,2),
    @OdemeTarihi DATE,
    @Aciklama NVARCHAR(150)
AS
BEGIN
    IF @Tutar <= 0
    BEGIN
        RAISERROR('Odeme tutari sifirdan buyuk olmalidir.', 16, 1);
        RETURN;
    END;

    INSERT INTO Odemeler (OgrenciID, Tutar, OdemeTarihi, Aciklama)
    VALUES (@OgrenciID, @Tutar, @OdemeTarihi, @Aciklama);
END;
GO

INSERT INTO Bolumler (BolumAdi) VALUES
(N'Bilgisayar Mühendisliği'), (N'Bilişim Sistemleri Mühendisliği'), (N'Endüstri Mühendisliği'),
(N'Elektrik Elektronik Mühendisliği'), (N'Makine Mühendisliği'), (N'İnşaat Mühendisliği'),
(N'İşletme'), (N'İktisat'), (N'Matematik'), (N'Mimarlık'),
(N'Hukuk'), (N'Tıp'), (N'Diş Hekimliği'), (N'Eczacılık'), (N'Psikoloji'),
(N'Sosyoloji'), (N'Tarih'), (N'Türk Dili ve Edebiyatı'), (N'Uluslararası İlişkiler'), (N'Maliye');

INSERT INTO OdaTipleri (TipAdi, AylikUcret) VALUES
(N'1 Kişilik', 4000), (N'2 Kişilik', 3000), (N'3 Kişilik', 2000), (N'4 Kişilik', 1000),
(N'1 Kişilik Balkonlu', 4500), (N'2 Kişilik Balkonlu', 3500),
(N'3 Kişilik Balkonlu', 2500), (N'4 Kişilik Balkonlu', 1500);

INSERT INTO Odalar (OdaNo, OdaTipID, Kapasite) VALUES
(N'101', 1, 1), (N'102', 2, 2), (N'103', 3, 3), (N'104', 4, 4),
(N'201', 5, 1), (N'202', 6, 2), (N'203', 7, 3), (N'204', 8, 4);

EXEC sp_OgrenciEkle N'10000000001', N'Ahmet', N'Yılmaz', N'05550000001', 1, 1;
EXEC sp_OgrenciEkle N'10000000002', N'Mehmet', N'Demir', N'05550000002', 2, 2;
EXEC sp_OgrenciEkle N'10000000003', N'Mustafa', N'Kaya', N'05550000003', 3, 2;
EXEC sp_OgrenciEkle N'10000000004', N'Ömer', N'Çelik', N'05550000004', 4, 3;
EXEC sp_OgrenciEkle N'10000000005', N'Emre', N'Şahin', N'05550000005', 5, 3;
EXEC sp_OgrenciEkle N'10000000006', N'Kerem', N'Yıldız', N'05550000006', 6, 3;
EXEC sp_OgrenciEkle N'10000000007', N'Can', N'Arslan', N'05550000007', 7, 4;
EXEC sp_OgrenciEkle N'10000000008', N'Berkay', N'Aydın', N'05550000008', 8, 4;
EXEC sp_OgrenciEkle N'10000000009', N'Burak', N'Koç', N'05550000009', 9, 4;
EXEC sp_OgrenciEkle N'10000000010', N'Yusuf', N'Öztürk', N'05550000010', 10, 4;
EXEC sp_OgrenciEkle N'10000000011', N'Enes', N'Kılıç', N'05550000011', 11, 5;
EXEC sp_OgrenciEkle N'10000000012', N'Kaan', N'Aslan', N'05550000012', 12, 6;
EXEC sp_OgrenciEkle N'10000000013', N'Mert', N'Yalçın', N'05550000013', 13, 6;
EXEC sp_OgrenciEkle N'10000000014', N'Arda', N'Şimşek', N'05550000014', 14, 7;
EXEC sp_OgrenciEkle N'10000000015', N'Eren', N'Polat', N'05550000015', 15, 7;
EXEC sp_OgrenciEkle N'10000000016', N'Umut', N'Aksoy', N'05550000016', 16, 7;
EXEC sp_OgrenciEkle N'10000000017', N'Onur', N'Çetin', N'05550000017', 17, 8;
EXEC sp_OgrenciEkle N'10000000018', N'Samet', N'Güneş', N'05550000018', 18, 8;
EXEC sp_OgrenciEkle N'10000000019', N'Furkan', N'Erdoğan', N'05550000019', 19, 8;
EXEC sp_OgrenciEkle N'10000000020', N'Halil', N'Yavuz', N'05550000020', 20, 8;

EXEC sp_OdemeEkle 1, 4000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 2, 3000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 3, 3000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 4, 2000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 5, 2000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 6, 2000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 7, 1000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 8, 1000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 9, 1000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 10, 1000, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 11, 4500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 12, 3500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 13, 3500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 14, 2500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 15, 2500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 16, 2500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 17, 1500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 18, 1500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 19, 1500, '2026-06-01', N'Haziran ayı yurt ücreti';
EXEC sp_OdemeEkle 20, 1500, '2026-06-01', N'Haziran ayı yurt ücreti';
GO
