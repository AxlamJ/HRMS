INSERT INTO [HRMS].[dbo].[FileMediaUplaod]
        (FileName, FilePath, ContentType, Size, ModuleName, ReferenceId, Extension,
         CreatedById, CreatedBy, CreatedDate, ModifiedById, ModifiedBy, ModifiedDate,
         IsActive, Status, Type, Title,Description)
      OUTPUT INSERTED.Id
        VALUES
        (@FileName, @FilePath, @ContentType, @Size, @ModuleName, @ReferenceId, @Extension,
         @CreatedById, @CreatedBy, @CreatedDate, @ModifiedById, @ModifiedBy, @ModifiedDate,
         @IsActive, @Status, @Type, @Title,@Description)