using Dapper;
using HrManagement.ApplicationSetting;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.IRepository;
using HrManagement.Models;

namespace HrManagement.Repository
{
    public class TrainingSessionRepository : ITrainingSessionRepository
    {
        private readonly DataContext _context;
        public TrainingSessionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<DtosTrainingCategoryPost>> WatchLesson(int? id)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
        SELECT 
            c.Id AS CategoryId,
            c.Title AS CategoryTitle,
            c.UserId,
            c.TrainingStructureId,
            c.ApprovedBy,
            c.IsActive,
            c.ItemType,
            c.Status AS CategoryStatus,
            c.Type AS CategoryType,

            p.Id AS PostId,
            p.CategoryId,
            p.Description,
            p.CreatedBy,
            p.Title AS PostTitle,
            p.CreatedDate,
            p.ModifiedBy,
            p.ModifiedDate,
            p.IsActive,
            p.Status AS PostStatus,
            p.Type AS PostType,

            mf.Id AS FileId,
            mf.FileName,
            mf.FilePath,
            mf.ContentType,
            mf.Size,
            mf.ModuleName,
            mf.ReferenceId,
            mf.Extension,
            mf.IsActive AS FileIsActive,
            mf.Type AS FileType

        FROM HRMS.dbo.TrainingStructureCategory c
        LEFT JOIN HRMS.dbo.TrainingPostDetails p ON p.CategoryId = c.Id
        LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = c.Id AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
        WHERE c.TrainingId = @TrainingId
        ";

            var categoryMap = new Dictionary<int, DtosTrainingCategoryPost>();

            await connection.QueryAsync<DtosTrainingCategoryPost, DtosTrainingPost, FileMediaUplaodDto, DtosTrainingCategoryPost>(
                sql,
                (category, post, file) =>
                {
                    if (!categoryMap.TryGetValue(category.Id, out var cat))
                    {
                        cat = new DtosTrainingCategoryPost
                        {
                            Id = category.Id,
                            Title = category.Title,
                            UserId = category.UserId,
                            TrainingStructureId = category.TrainingStructureId,
                            ApprovedBy = category.ApprovedBy,
                            Status = category.Status,
                            Type = category.Type,
                            Posts = new List<DtosTrainingPost>(),
                            FileMediaUplaod = new List<FileMediaUplaodDto>()
                        };
                        categoryMap[category.Id] = cat;
                    }

                    if (post != null && post.Id > 0 && !cat.Posts.Any(p => p.Id == post.Id))
                    {
                        cat.Posts.Add(new DtosTrainingPost
                        {
                            Id = post.Id,
                            CategoryId = post.CategoryId,
                            Title = post.Title,
                            Description = post.Description,
                            CreatedBy = post.CreatedBy,
                            CreatedDate = post.CreatedDate,
                            ModifiedBy = post.ModifiedBy,
                            ModifiedDate = post.ModifiedDate,
                            IsActive = post.IsActive,
                            Status = post.Status,
                            Type = post.Type
                        });
                    }

                    if (file != null && file.FileId > 0 && !cat.FileMediaUplaod.Any(f => f.FileId == file.FileId))
                    {
                        cat.FileMediaUplaod.Add(file);
                    }

                    return cat;
                },
                new { TrainingId = id, ModuleName = ApplicationSettings.ModuleName.PostDetail },
                splitOn: "PostId,FileId"
            );

            var finalResult = categoryMap.Values.ToList();

            if (!finalResult.Any())
            {
                return finalResult;
            }
            return null;

        }


    }
}
