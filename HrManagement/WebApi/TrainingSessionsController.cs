using Dapper;
using HrManagement.ApplicationSetting;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.Helpers;
using HrManagement.IRepository;
using HrManagement.Models;
using HrManagement.Repository;
using HrManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingSessionsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        private readonly ITrainingRepository _trainingRepository;
        private readonly IFileUploadService _IfileUploadService;
        private readonly ITrainingSessionRepository _trainingSessionRepository;
        private readonly IPermissionRepository _permissionRepository;
        public TrainingSessionsController(DataContext context, Email email, ITrainingRepository trainingRepository, IFileUploadService IfileUploadService, ITrainingSessionRepository trainingSessionRepository, IPermissionRepository permissionRepository)
        {
            _context = context;
            _email = email;
            _trainingRepository = trainingRepository;
            _IfileUploadService = IfileUploadService;
            _trainingSessionRepository = trainingSessionRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpPost("WatchLesson1")]
        public async Task<IActionResult> WatchLessone(int? id = null)
        {
            try
            {
                using var connection = _context.CreateConnection();

                if (id == null || id <= 1)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
                }
                var result = await _trainingSessionRepository.WatchLesson(id);
                if (result is not null)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Training fetched successfully.",
                        Data = result
                    });
                }


                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }


        }

        [HttpPost("WatchLesson")]
        public async Task<IActionResult> WatchLesson(int id)
        {

            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                using var connection = _context.CreateConnection();

                string sql = @"
                    SELECT 
                        t.TrainingId, t.Title AS TrainingTitle,
                        t.IsActive AS TrainingIsActive,
                    
                        s.TrainingStructureId, s.Title AS StructureTitle, s.Type AS StructureType, s.Status AS SStatus,
                    
                    
                        c.Id AS CategoryId, c.Title AS CategoryTitle, c.UserId AS CategoryUserId, 
                        c.TrainingStructureId AS CategoryTrainingStructureId, c.ApprovedBy AS CategoryApprovedBy, 
                        c.Type AS CategoryType, c.SubCategoryId AS CategorySubCategoryId, c.Status AS CStatus
                    
                    FROM Trainings t
                    LEFT JOIN TrainingStructure s ON t.TrainingId = s.TrainingId
                    LEFT JOIN TrainingStructureCategory c ON s.TrainingStructureId = c.TrainingStructureId
                    WHERE t.TrainingId = @TrainingId
                    ORDER BY s.TrainingStructureId, c.Id";

                var trainingMap = new Dictionary<int, TrainingLessonSession>();

                var result = await connection.QueryAsync<TrainingLessonSession, TrainingStructureLessonSession, TrainingStructureCategoryLessonSession, TrainingLessonSession>(
                    sql,
                    (training, structure, category) =>
                    {
                        if (!trainingMap.TryGetValue(training.TrainingId, out var trainingEntry))
                        {
                            trainingEntry = new TrainingLessonSession
                            {
                                TrainingId = training.TrainingId,
                                TrainingTitle = training.TrainingTitle,
                                Structures = new List<TrainingStructureLessonSession>()
                            };
                            trainingMap.Add(trainingEntry.TrainingId, trainingEntry);
                        }

                        if (structure != null && structure.TrainingStructureId != 0)
                        {
                            var existingStructure = trainingEntry.Structures.FirstOrDefault(s => s.TrainingStructureId == structure.TrainingStructureId);
                            if (existingStructure == null)
                            {
                                existingStructure = new TrainingStructureLessonSession
                                {
                                    TrainingStructureId = structure.TrainingStructureId,
                                    StructureTitle = structure.StructureTitle,
                                    SStatus = structure.SStatus,
                                    StructureType = structure.StructureType,
                                    Categories = new List<TrainingStructureCategoryLessonSession>()
                                };
                                trainingEntry.Structures.Add(existingStructure);
                            }

                            if (category != null && category.CategoryId != 0)
                            {
                                // Avoid duplicate categories
                                if (!existingStructure.Categories.Any(c => c.CategoryId == category.CategoryId))
                                {
                                    existingStructure.Categories.Add(new TrainingStructureCategoryLessonSession
                                    {
                                        CategoryId = category.CategoryId,
                                        CategoryTitle = category.CategoryTitle,
                                        CategoryUserId = category.CategoryUserId,
                                        CategoryTrainingStructureId = category.CategoryTrainingStructureId,
                                        CategoryApprovedBy = category.CategoryApprovedBy,
                                        CStatus=category.CStatus,
                                        CategoryType = category.CategoryType,
                                        CategorySubCategoryId = category.CategorySubCategoryId
                                    });
                                }
                            }
                        }

                        return trainingEntry;
                    },
                    new { TrainingId = id },
                    splitOn: "TrainingStructureId,CategoryId"
                );

                var trainingDto = trainingMap.Values.FirstOrDefault();

                if (trainingDto == null)
                    return NotFound(new { StatusCode = 404, Message = "Training not found." });

                foreach (var structure in trainingDto.Structures)
                {
                    // Group all subcategories under their parents
                    var flatCategories = structure.Categories.ToList();

                    foreach (var cat in flatCategories.Where(c => c.CategorySubCategoryId != null))
                    {
                        var parent = structure.Categories.FirstOrDefault(p => p.CategoryId == cat.CategorySubCategoryId);
                        if (parent != null)
                        {
                            var subCat = new DtosTrainingSubCategoryLessonSession
                            {
                                CategoryId = cat.CategoryId,
                                CategoryTitle = cat.CategoryTitle,
                                CategoryUserId = cat.CategoryUserId,
                                CategoryTrainingStructureId = cat.CategoryTrainingStructureId,
                                CategoryApprovedBy = cat.CategoryApprovedBy,
                                CStatus = cat.CStatus,
                                CategoryType = cat.CategoryType,
                                CategorySubCategoryId = cat.CategorySubCategoryId
                            };

                            parent.TrainingSubCategories.Add(subCat);
                            structure.Categories.Remove(cat);
                        }
                    }
                }
                if (trainingDto is not null)
                {

                        return Ok(new
                        {
                            StatusCode = 200,
                            Message = "Training fetched successfully.",
                            Data = trainingDto
                        });
                }
                return NotFound(new { StatusCode = 404, Message = "Training not found." });

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("EmployeeWatchLesson")]
        public async Task<IActionResult> EmployeeWatchLesson(int id)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                using var connection = _context.CreateConnection();

                string sql = @"
                    SELECT 
                        t.TrainingId, t.Title AS TrainingTitle,
                        t.IsActive AS TrainingIsActive,
                    
                        s.TrainingStructureId, s.Title AS StructureTitle, s.Type AS StructureType, s.Status AS SStatus,
                    
                    
                        c.Id AS CategoryId, c.Title AS CategoryTitle, c.UserId AS CategoryUserId, 
                        c.TrainingStructureId AS CategoryTrainingStructureId, c.ApprovedBy AS CategoryApprovedBy, 
                        c.Type AS CategoryType, c.SubCategoryId AS CategorySubCategoryId, c.Status AS CStatus
                    
                    FROM Trainings t
                    LEFT JOIN TrainingStructure s ON t.TrainingId = s.TrainingId
                    LEFT JOIN TrainingStructureCategory c ON s.TrainingStructureId = c.TrainingStructureId
                    WHERE t.TrainingId = @TrainingId AND t.IsActive=1 AND s.Status='1' AND c.Status='1'
                    ORDER BY s.TrainingStructureId, c.Id";

                var trainingMap = new Dictionary<int, TrainingLessonSession>();

                var result = await connection.QueryAsync<TrainingLessonSession, TrainingStructureLessonSession, TrainingStructureCategoryLessonSession, TrainingLessonSession>(
                    sql,
                    (training, structure, category) =>
                    {
                        if (!trainingMap.TryGetValue(training.TrainingId, out var trainingEntry))
                        {
                            trainingEntry = new TrainingLessonSession
                            {
                                TrainingId = training.TrainingId,
                                TrainingTitle = training.TrainingTitle,
                                Structures = new List<TrainingStructureLessonSession>()
                            };
                            trainingMap.Add(trainingEntry.TrainingId, trainingEntry);
                        }

                        if (structure != null && structure.TrainingStructureId != 0)
                        {
                            var existingStructure = trainingEntry.Structures.FirstOrDefault(s => s.TrainingStructureId == structure.TrainingStructureId);
                            if (existingStructure == null)
                            {
                                existingStructure = new TrainingStructureLessonSession
                                {
                                    TrainingStructureId = structure.TrainingStructureId,
                                    StructureTitle = structure.StructureTitle,
                                    SStatus = structure.SStatus,
                                    StructureType = structure.StructureType,
                                    Categories = new List<TrainingStructureCategoryLessonSession>()
                                };
                                trainingEntry.Structures.Add(existingStructure);
                            }

                            if (category != null && category.CategoryId != 0)
                            {
                                // Avoid duplicate categories
                                if (!existingStructure.Categories.Any(c => c.CategoryId == category.CategoryId))
                                {
                                    existingStructure.Categories.Add(new TrainingStructureCategoryLessonSession
                                    {
                                        CategoryId = category.CategoryId,
                                        CategoryTitle = category.CategoryTitle,
                                        CategoryUserId = category.CategoryUserId,
                                        CategoryTrainingStructureId = category.CategoryTrainingStructureId,
                                        CategoryApprovedBy = category.CategoryApprovedBy,
                                        CStatus = category.CStatus,
                                        CategoryType = category.CategoryType,
                                        CategorySubCategoryId = category.CategorySubCategoryId
                                    });
                                }
                            }
                        }

                        return trainingEntry;
                    },
                    new { TrainingId = id },
                    splitOn: "TrainingStructureId,CategoryId"
                );

                var trainingDto = trainingMap.Values.FirstOrDefault();

                if (trainingDto == null)
                    return NotFound(new { StatusCode = 404, Message = "Training not found." });

                foreach (var structure in trainingDto.Structures)
                {
                    // Group all subcategories under their parents
                    var flatCategories = structure.Categories.ToList();

                    foreach (var cat in flatCategories.Where(c => c.CategorySubCategoryId != null))
                    {
                        var parent = structure.Categories.FirstOrDefault(p => p.CategoryId == cat.CategorySubCategoryId);
                        if (parent != null)
                        {
                            var subCat = new DtosTrainingSubCategoryLessonSession
                            {
                                CategoryId = cat.CategoryId,
                                CategoryTitle = cat.CategoryTitle,
                                CategoryUserId = cat.CategoryUserId,
                                CategoryTrainingStructureId = cat.CategoryTrainingStructureId,
                                CategoryApprovedBy = cat.CategoryApprovedBy,
                                CStatus = cat.CStatus,
                                CategoryType = cat.CategoryType,
                                CategorySubCategoryId = cat.CategorySubCategoryId
                            };

                            parent.TrainingSubCategories.Add(subCat);
                            structure.Categories.Remove(cat);
                        }
                    }
                }
                if (trainingDto is not null)
                {

                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Training fetched successfully.",
                        Data = trainingDto
                    });
                }
                return NotFound(new { StatusCode = 404, Message = "Training not found." });

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("GetTrainingDetails")]
        public async Task<IActionResult> GetTrainingDetails(int trainingId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                string sql = @"SELECT 
                           t.TrainingId, t.Title AS Title, t.UserId, t.ApprovedBy, t.CreatedDate, t.Status, t.Duration,

                           d.TrainingsDetailId, d.Title, d.Description, d.InstructorHeading,
                           d.InstructorName, d.InstructorTitle, d.InstructorBio, d.Status, d.Duration,

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

                            FROM Trainings t
                            LEFT JOIN TrainingsDetails d ON t.TrainingId = d.TrainingId
                            LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = t.TrainingId 
                                AND mf.ModuleName = @ModuleName 
                                AND mf.IsActive = 1
                            WHERE t.TrainingId = @TrainingId
                           ";


                var trainingMap = new Dictionary<int, DtosTraining>();

                var result = await connection.QueryAsync<DtosTraining, DtosTrainingsDetail, FileMediaUplaodDto, DtosTraining>(
                    sql,
                    (training, detail, file) =>
                    {
                        if (!trainingMap.TryGetValue(training.TrainingId, out var dto))
                        {
                            dto = training;
                            trainingMap[dto.TrainingId] = dto;
                        }

                        if (detail?.TrainingsDetailId > 0 &&
                            !dto.Details.Any(d => d.TrainingsDetailId == detail.TrainingsDetailId))
                        {
                            dto.Details.Add(detail);
                        }

                        if (file?.FileId > 0 && !dto.FileMediaUplaod.Any(f => f.FileId == file.FileId))
                        {
                            dto.FileMediaUplaod.Add(file);
                        }

                        return dto;
                    },
                    new { TrainingId = trainingId, ModuleName = ApplicationSettings.ModuleName.TrainingDetail },
                    splitOn: "TrainingsDetailId,FileId"
                );

                var trainingDto = trainingMap.Values.FirstOrDefault();

                if (trainingDto == null)
                    return NotFound(new { StatusCode = 404, Message = "Training not found." });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Training fetched successfully.",
                    Data = trainingDto
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

    }
}
