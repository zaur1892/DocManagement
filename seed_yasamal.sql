-- =============================================
-- 1. YASAMAL RAYONU VƏ MƏNTƏQƏLƏRİ
-- =============================================
DECLARE @bakuId INT;
SELECT @bakuId = Id FROM Cities WHERE Name = N'Bakı';

IF @bakuId IS NULL
BEGIN
    PRINT N'Xəta: Bakı tapılmadı!';
    RETURN;
END

-- Yasamal rayonu
IF NOT EXISTS (SELECT 1 FROM Districts WHERE Name = N'Yasamal' AND CityId = @bakuId)
BEGIN
    INSERT INTO Districts (CityId, Name, NameRu, NameEn, SortOrder)
    VALUES (@bakuId, N'Yasamal', N'Ясамал', N'Yasamal', 12);
    PRINT N'Yasamal rayonu əlavə edildi.';
END
ELSE
    PRINT N'Yasamal artıq mövcuddur.';

DECLARE @yasamalId INT;
SELECT @yasamalId = Id FROM Districts WHERE Name = N'Yasamal' AND CityId = @bakuId;

-- Yasamal məntəqələri
IF NOT EXISTS (SELECT 1 FROM Settlements WHERE DistrictId = @yasamalId)
BEGIN
    INSERT INTO Settlements (DistrictId, Name, NameRu)
    VALUES 
        (@yasamalId, N'Montin qəs.', N'пос. Монтин'),
        (@yasamalId, N'Dağüstü qəs.', N'пос. Дагюсту'),
        (@yasamalId, N'9-cu mkr.', N'9-й микрорайон'),
        (@yasamalId, N'Yeni Yasamal', N'Новый Ясамал'),
        (@yasamalId, N'Elmlər mkr.', N'микр. Науки'),
        (@yasamalId, N'Yasamal qəs.', N'пос. Ясамал');
    PRINT N'Yasamal məntəqələri əlavə edildi.';
END
ELSE
    PRINT N'Yasamal məntəqələri artıq mövcuddur.';

-- =============================================
-- 2. DEMO İSTİFADƏÇİLƏR (şifrə: Demo123!)
-- =============================================
-- AspNetUsers cədvəlinə birbaşa daxil etmək mümkün deyil çünki password hash lazımdır.
-- Bunu C# kodundan edəcəyik (DatabaseSeeder vasitəsilə).

-- =============================================
-- 3. RAYON YOXLAMASI
-- =============================================
SELECT d.Id, d.Name, d.SortOrder, COUNT(s.Id) AS [Məntəqə sayı]
FROM Districts d
LEFT JOIN Settlements s ON s.DistrictId = d.Id
WHERE d.CityId = @bakuId
GROUP BY d.Id, d.Name, d.SortOrder
ORDER BY d.SortOrder;
