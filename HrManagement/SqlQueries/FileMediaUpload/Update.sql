 UPDATE [HRMS].[dbo].[FileMediaUplaod]
        SET FileName = @FileName,
            FilePath = @FilePath,
            ContentType = @ContentType,
            Size = @Size,
            Extension = @Extension,
            ModifiedById = @ModifiedById,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = @ModifiedDate,
            Type = @Type,
            Description = Description
        WHERE Id = @Id