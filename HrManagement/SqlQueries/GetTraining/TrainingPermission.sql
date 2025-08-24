SELECT
    -- Training (root)
    t.TrainingId,
    t.Title AS Title,
    t.UserId,
    t.ApprovedBy,
    t.CreatedDate,
    t.Status,
    t.Duration,
    t.ItemType AS TItemType,

    -- Details (nested array)
    d.TrainingsDetailId AS [Details.TrainingsDetailId],
    d.Title AS [Details.Title],
    d.Description AS [Details.Description],
    d.InstructorHeading AS [Details.InstructorHeading],
    d.InstructorName AS [Details.InstructorName],
    d.InstructorTitle AS [Details.InstructorTitle],
    d.InstructorBio AS [Details.InstructorBio],
    d.Status AS [Details.Status],
    d.Duration AS [Details.Duration],

    -- FileMediaUploads (nested array)
    mf.Id AS [FileMediaUploads.FileId],
    mf.FileName AS [FileMediaUploads.FileName],
    mf.FilePath AS [FileMediaUploads.FilePath],
    mf.ContentType AS [FileMediaUploads.ContentType],
    mf.Size AS [FileMediaUploads.Size],
    mf.ModuleName AS [FileMediaUploads.ModuleName],
    mf.ReferenceId AS [FileMediaUploads.ReferenceId],
    mf.Extension AS [FileMediaUploads.Extension],
    mf.IsActive AS [FileMediaUploads.FileIsActive],
    mf.Type AS [FileMediaUploads.FileType],

    -- Structures (nested array)
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.TrainingStructureId ELSE NULL END AS [Structures.TrainingStructureId],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Title ELSE NULL END AS [Structures.Title],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Type ELSE NULL END AS [Structures.Type],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.UserId ELSE NULL END AS [Structures.UserId],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.ApprovedBy ELSE NULL END AS [Structures.ApprovedBy],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Status ELSE NULL END AS [Structures.Status],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Duration ELSE NULL END AS [Structures.Duration],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.TrainingId ELSE NULL END AS [Structures.TrainingId],
    CASE WHEN p_s.PermissionId IS NOT NULL THEN s.ItemType ELSE NULL END AS [Structures.SItemType],

    -- Structures.Categories (nested under Structures)
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Id ELSE NULL END AS [Structures.Categories.Id],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Title ELSE NULL END AS [Structures.Categories.Title],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.UserId ELSE NULL END AS [Structures.Categories.UserId],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.TrainingStructureId ELSE NULL END AS [Structures.Categories.TrainingStructureId],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.ApprovedBy ELSE NULL END AS [Structures.Categories.ApprovedBy],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Status ELSE NULL END AS [Structures.Categories.Status],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Type ELSE NULL END AS [Structures.Categories.Type],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.SubCategoryId ELSE NULL END AS [Structures.Categories.SubCategoryId],
    CASE WHEN p_c.PermissionId IS NOT NULL THEN c.ItemType ELSE NULL END AS [Structures.Categories.CItemType]

FROM Trainings t
LEFT JOIN TrainingsDetails d ON t.TrainingId = d.TrainingId
LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = t.TrainingId 
    AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
INNER JOIN Permissions p_t ON p_t.ItemId = t.TrainingId 
    AND p_t.ItemType = t.ItemType AND p_t.UserId = @UserId 
    AND p_t.IsAssigned = 1 AND p_t.IsActive = 1
LEFT JOIN TrainingStructure s ON s.TrainingId = t.TrainingId
LEFT JOIN Permissions p_s ON p_s.ItemId = s.TrainingStructureId 
    AND p_s.ItemType = s.ItemType AND p_s.UserId = @UserId 
    AND p_s.IsAssigned = 1 AND p_s.IsActive = 1
LEFT JOIN TrainingStructureCategory c ON c.TrainingStructureId = s.TrainingStructureId
LEFT JOIN Permissions p_c ON p_c.ItemId = c.Id 
    AND p_c.ItemType = c.ItemType AND p_c.UserId = @UserId 
    AND p_c.IsAssigned = 1 AND p_c.IsActive = 1
WHERE t.TrainingId = @TrainingId 
    AND p_t.PermissionId IS NOT NULL 
    AND (s.TrainingStructureId IS NULL OR p_s.PermissionId IS NOT NULL)
ORDER BY s.TrainingStructureId, c.Id
FOR JSON PATH, INCLUDE_NULL_VALUES, ROOT('PermissionTraining');
