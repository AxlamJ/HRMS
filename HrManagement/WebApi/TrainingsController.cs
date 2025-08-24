using Dapper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.ApplicationSetting;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.Helpers;
using HrManagement.IRepository;
using HrManagement.Models;
using HrManagement.Repository;
using HrManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Newtonsoft.Json;

namespace HrManagement.WebApi
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class TrainingsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        private readonly ITrainingRepository _trainingRepository;
        private readonly IFileUploadService _IfileUploadService;

        private readonly IPermissionRepository _permissionRepository;
        public TrainingsController(DataContext context, Email email, ITrainingRepository trainingRepository, IFileUploadService IfileUploadService, IPermissionRepository permissionRepository)
        {
            _context = context;
            _email = email;
            _trainingRepository = trainingRepository;
            _IfileUploadService = IfileUploadService;
            _permissionRepository = permissionRepository;
        }


        [HttpPost("UpsertTraining")]
        public async Task<IActionResult> UpsertTraining(Training? training)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");

                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                using var connection = _context.CreateConnection();

                if (training != null)
                {
                    if (training.TrainingId == 0) // Insert
                    {
                        const string insertQuery = @"
         INSERT INTO Trainings (
             Title,
             UserId,
             ApprovedBy,
             CreatedById,
             CreatedBy,
             CreatedDate,
             ModifiedById,
             ModifiedBy,
             ModifiedDate,
             IsActive,
             Status,
             Duration,
             IsApproved
         ) 
        OUTPUT INSERTED.TrainingId
        VALUES (
             @Title,
             @UserId,
             @ApprovedBy,
             @CreatedById,
             @CreatedBy,
             @CreatedDate,
             @ModifiedById,
             @ModifiedBy,
             @ModifiedDate,
             @IsActive,
             @Status,
             @Duration,
            @IsApproved
         );

         SELECT SCOPE_IDENTITY();";

                        training.CreatedById = loggedInUserId;
                        training.CreatedBy = fullName;
                        training.CreatedDate = DateTime.UtcNow;

                        training.ModifiedById = loggedInUserId;
                        training.ModifiedBy = fullName;
                        training.ModifiedDate = DateTime.UtcNow;
                        training.IsApproved = false;

                        training.IsActive = true;

                        var trainingId = await connection.ExecuteScalarAsync<int>(insertQuery, training);
                        if (trainingId > 0)
                        {

                            ///
                            var trainingsDetail = new TrainingsDetail();
                            trainingsDetail.CreatedById = loggedInUserId;
                            trainingsDetail.CreatedBy = training.CreatedBy;
                            trainingsDetail.CreatedDate = DateTime.UtcNow;
                            trainingsDetail.ModifiedById = loggedInUserId;
                            trainingsDetail.ModifiedBy = training.CreatedBy;
                            trainingsDetail.ModifiedDate = DateTime.UtcNow;
                            trainingsDetail.Title = training.Title;
                            trainingsDetail.Status = ApplicationSettings.TariningStatus.Draft;
                            trainingsDetail.TrainingId = trainingId;



                            trainingsDetail.IsActive = true;

                            var trainingdetails = await _trainingRepository.UpsertTrainingDetail(trainingsDetail);

                            var trainingStructure = new TrainingStructure();
                            ///
                            trainingStructure.Title = "Course Completion Credential";
                            trainingStructure.Type = ApplicationSettings.TrainingType.CredentialCategory;
                            trainingStructure.CreatedById = training.UserId;
                            trainingStructure.CreatedBy = training.CreatedBy;

                            trainingStructure.CreatedDate = DateTime.UtcNow;

                            trainingStructure.ModifiedById = loggedInUserId;
                            trainingStructure.ModifiedBy = training.CreatedBy;

                            trainingStructure.ModifiedDate = DateTime.UtcNow;
                            trainingStructure.TrainingId = trainingId;
                            var IsCreated = await _trainingRepository.UpsertTrainingStructure(trainingStructure);

                            trainingStructure.Title = "Blank Category";
                            trainingStructure.Type = ApplicationSettings.TrainingType.BlankCategory;
                            trainingStructure.Status = ApplicationSettings.TariningStatus.Draft;

                            trainingStructure.IsActive = true;
                            var BlankCategory = await _trainingRepository.UpsertTrainingStructure(trainingStructure);
                        }

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "Training created successfully.",
                            Data = new { Id = trainingId }
                        });
                    }
                    else // Update
                    {
                        const string updateQuery = @"
         UPDATE Trainings SET
             Title = @Title,
             UserId = @UserId,
             ApprovedBy = @ApprovedBy,
             ModifiedById = @ModifiedById,
             ModifiedBy = @ModifiedBy,
             ModifiedDate = @ModifiedDate,
             IsActive = @IsActive,
             Status = @Status,
             Duration = @Duration
         WHERE TrainingId = @TrainingId";

                        training.ModifiedById = loggedInUserId;
                        training.ModifiedBy = fullName;
                        training.ModifiedDate = DateTime.UtcNow;
                        training.IsActive = true;

                        await connection.ExecuteAsync(updateQuery, training);

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "Training updated successfully.",
                            Data = new { Id = training.TrainingId }
                        });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("DeleteTraining")]
        public async Task<IActionResult> DeleteTraining(int? trainingId)
        {
            try
            {
                var training = new Training();
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";
                using var connection = _context.CreateConnection();

                var UserRoleName = HttpContext.Session.GetString("UserRoleName");
                if (UserRoleName?.ToLower() == "super admin")
                {
                    training.IsApproved = true;
                }
                if (training != null)
                {

                    const string updateQuery = @"
                     UPDATE Trainings SET
                     UserId = @UserId,
                     ModifiedById = @ModifiedById,
                     ModifiedBy = @ModifiedBy,
                     ModifiedDate = @ModifiedDate,
                     IsActive = @IsActive,
                     IsApproved=@IsApproved
                     WHERE TrainingId = @TrainingId";
                    training.ModifiedById = loggedInUserId;
                    training.ModifiedBy = fullName;
                    training.ModifiedDate = DateTime.UtcNow;
                    training.IsActive = false;
                    training.TrainingId = (int)trainingId;
                    await connection.ExecuteAsync(updateQuery, training);
                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        Message = "Training Deleted successfully.",
                        Data = new { Id = training.TrainingId }
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


        [HttpPost("CloneFullTraining")]
        public async Task<IActionResult> CloneFullTrainingAsync(int? trainingId)
        {
            try
            {
                if (trainingId == null || trainingId == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
                }
                var training = new Training();
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                var result = await _trainingRepository.CloneFullTrainingAsync((int)trainingId);
                if (result > 0)
                {
                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        Message = "Training Clone successfully.",
                        Data = new { Id = training.TrainingId }
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


        [HttpPost("UpsertTrainingStructure")]
        public async Task<IActionResult> UpsertTrainingStructure(TrainingStructure trainingStructure)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");

                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                using var connection = _context.CreateConnection();

                if (trainingStructure != null)
                {
                    if (trainingStructure.TrainingStructureId == 0) // Insert
                    {
                        const string insertQuery = @"
                           INSERT INTO TrainingStructure (
                               Title,
                               Type,
                               UserId,
                               ApprovedBy,
                               CreatedById,
                               CreatedBy,
                               CreatedDate,
                               ModifiedById,
                               ModifiedBy,
                               ModifiedDate,
                               IsActive,
                               Status,
                               Duration,
                               TrainingId
                           ) VALUES (
                               @Title,
                               @Type,
                               @UserId,
                               @ApprovedBy,
                               @CreatedById,
                               @CreatedBy,
                               @CreatedDate,
                               @ModifiedById,
                               @ModifiedBy,
                               @ModifiedDate,
                               @IsActive,
                               @Status,
                               @Duration,
                               @TrainingId
                           );
                           
                           SELECT SCOPE_IDENTITY();";

                        trainingStructure.CreatedById = loggedInUserId;
                        trainingStructure.CreatedBy = fullName;
                        trainingStructure.CreatedDate = DateTime.UtcNow;

                        trainingStructure.ModifiedById = loggedInUserId;
                        trainingStructure.ModifiedBy = fullName;
                        trainingStructure.ModifiedDate = DateTime.UtcNow;
                        trainingStructure.Status = ApplicationSettings.TariningStatus.Draft;
                        trainingStructure.IsActive = true;

                        var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, trainingStructure);

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "TrainingStructure created successfully.",
                            Data = new { Id = insertedId }
                        });
                    }
                    else // Update
                    {
                        const string updateQuery = @"
                        UPDATE TrainingStructure SET
                            Title = @Title,
                            Type = @Type,
                            UserId = @UserId,
                            ApprovedBy = @ApprovedBy,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            IsActive = @IsActive,
                            Status = @Status,
                            Duration = @Duration,
                            TrainingId = @TrainingId
                        WHERE TrainingStructureId = @TrainingStructureId";

                        trainingStructure.ModifiedById = loggedInUserId;
                        trainingStructure.ModifiedBy = fullName;
                        trainingStructure.ModifiedDate = DateTime.UtcNow;
                        trainingStructure.IsActive = true;

                        await connection.ExecuteAsync(updateQuery, trainingStructure);

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "TrainingStructure updated successfully.",
                            Data = new { Id = trainingStructure.TrainingStructureId }
                        });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("UpsertTrainingsDetail")]
        public async Task<IActionResult> UpsertTrainingsDetail(TrainingsDetail trainingsDetail)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                using var connection = _context.CreateConnection();

                if (trainingsDetail != null)
                {
                    if (trainingsDetail.TrainingsDetailId == 0) // Insert
                    {
                        const string insertQuery = @"
                    INSERT INTO TrainingsDetails (
                        Title,
                        TrainingId,
                        Description,
                        InstructorHeading,
                        InstructorName,
                        InstructorTitle,
                        InstructorBio,
                        CreatedById,
                        CreatedBy,
                        CreatedDate,
                        ModifiedById,
                        ModifiedBy,
                        ModifiedDate,
                        IsActive
                    ) VALUES (
                        @Title,
                        @TrainingId,
                        @Description,
                        @InstructorHeading,
                        @InstructorName,
                        @InstructorTitle,
                        @InstructorBio,
                        @CreatedById,
                        @CreatedBy,
                        @CreatedDate,
                        @ModifiedById,
                        @ModifiedBy,
                        @ModifiedDate,
                        @IsActive
                    );

                    SELECT SCOPE_IDENTITY();";

                        trainingsDetail.CreatedById = loggedInUserId;
                        trainingsDetail.CreatedBy = fullName;
                        trainingsDetail.CreatedDate = DateTime.UtcNow;

                        trainingsDetail.ModifiedById = loggedInUserId;
                        trainingsDetail.ModifiedBy = fullName;
                        trainingsDetail.ModifiedDate = DateTime.UtcNow;

                        trainingsDetail.IsActive = true;

                        var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, trainingsDetail);

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "Product created successfully.",
                            Data = new { Id = insertedId }
                        });
                    }
                    else // Update
                    {
                        const string updateQuery = @"
                    UPDATE TrainingsDetails SET
                        Title = @Title,
                        Description = @Description,
                        InstructorHeading = @InstructorHeading,
                        InstructorName = @InstructorName,
                        InstructorTitle = @InstructorTitle,
                        InstructorBio = @InstructorBio,
                        ModifiedById = @ModifiedById,
                        ModifiedBy = @ModifiedBy,
                        ModifiedDate = @ModifiedDate,
                        IsActive = @IsActive
                        WHERE TrainingsDetailId = @TrainingsDetailId";

                        trainingsDetail.ModifiedById = loggedInUserId;
                        trainingsDetail.ModifiedBy = fullName;
                        trainingsDetail.ModifiedDate = DateTime.UtcNow;

                        trainingsDetail.IsActive = true;

                        await connection.ExecuteAsync(updateQuery, trainingsDetail);

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "Product updated successfully.",
                            Data = new { Id = trainingsDetail.TrainingsDetailId }
                        });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }




        //        [HttpPost("GetTrainingWithDetails")]
        //        public async Task<IActionResult> GetTrainingWithDetails(int trainingId)
        //        {
        //            try
        //            {
        //                using var connection = _context.CreateConnection();

        //                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        //                int userRole = Convert.ToInt32(HttpContext.Session.GetString("UserRole"));
        //                bool isSuperAdmin = userRole == 1;
        //                string sql = string.Empty;
        //                if (userRole == 1)
        //                {
        //                    sql = @"
        //                SELECT 
        //                t.TrainingId, t.Title AS Title, t.UserId, t.ApprovedBy, t.CreatedDate, t.Status, t.Duration,
        //                t.ItemType as TItemType,

        //                d.TrainingsDetailId, d.Title, d.Description, d.InstructorHeading,
        //                d.InstructorName, d.InstructorTitle, d.InstructorBio, d.Status, d.Duration,

        //                s.TrainingStructureId, s.Title, s.Type, s.UserId, 
        //                s.ApprovedBy, s.Status, s.Duration, s.TrainingId,
        //                s.ItemType as SItemType,

        //                c.Id, c.Title, c.UserId, 
        //                c.TrainingStructureId, c.ApprovedBy, c.Status, c.Type, c.SubCategoryId,
        //                c.ItemType as CItemType,

        //                mf.Id AS FileId,
        //                mf.FileName,
        //                mf.FilePath,
        //                mf.ContentType,
        //                mf.Size,
        //                mf.ModuleName,
        //                mf.ReferenceId,
        //                mf.Extension,
        //                mf.IsActive AS FileIsActive,
        //                mf.Type AS FileType

        //            FROM Trainings t
        //            LEFT JOIN TrainingsDetails d ON t.TrainingId = d.TrainingId
        //            LEFT JOIN TrainingStructure s ON t.TrainingId = s.TrainingId
        //            LEFT JOIN TrainingStructureCategory c ON s.TrainingStructureId = c.TrainingStructureId  
        //            LEFT JOIN FileMediaUplaod mf 
        //                ON mf.ReferenceId = t.TrainingId AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
        //            WHERE t.TrainingId = @TrainingId
        //            ORDER BY s.TrainingStructureId ASC, c.Id ASC";
        //                }
        //                else
        //                {
        //                    sql = @"SELECT
        //  -- Training columns
        //  t.TrainingId,
        //  t.Title AS TrainingTitle,
        //  t.UserId,
        //  t.ApprovedBy,
        //  t.CreatedDate,
        //  t.Status,
        //  t.Duration,
        //  t.ItemType AS TItemType,

        // d.TrainingsDetailId, d.Title, d.Description, d.InstructorHeading,
        // d.InstructorName, d.InstructorTitle, d.InstructorBio, d.Status, d.Duration,

        // mf.Id AS FileId,
        // mf.FileName,
        // mf.FilePath,
        // mf.ContentType,
        // mf.Size,
        // mf.ModuleName,
        // mf.ReferenceId,
        // mf.Extension,
        // mf.IsActive AS FileIsActive,
        // mf.Type AS FileType,


        // s.TrainingStructureId AS SplitStructureId,
        // c.Id AS SplitCategoryId,

        //  -- TrainingStructure columns (only if permission exists)
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.TrainingStructureId ELSE NULL END AS TrainingStructureId,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Title ELSE NULL END AS StructureTitle,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Type ELSE NULL END AS StructureType,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.UserId ELSE NULL END AS StructureUserId,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.ApprovedBy ELSE NULL END AS StructureApprovedBy,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Status ELSE NULL END AS StructureStatus,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.Duration ELSE NULL END AS StructureDuration,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.TrainingId ELSE NULL END AS StructureTrainingId,
        //  CASE WHEN p_s.PermissionId IS NOT NULL THEN s.ItemType ELSE NULL END AS SItemType,
        //  c.Id AS SplitCategoryId,
        //  -- Category columns (only if permission exists)
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Id ELSE NULL END AS CategoryId,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Title ELSE NULL END AS CategoryTitle,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.UserId ELSE NULL END AS CategoryUserId,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.TrainingStructureId ELSE NULL END AS CategoryTrainingStructureId,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.ApprovedBy ELSE NULL END AS CategoryApprovedBy,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Status ELSE NULL END AS CategoryStatus,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.Type ELSE NULL END AS CategoryType,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.SubCategoryId ELSE NULL END AS SubCategoryId,
        //  CASE WHEN p_c.PermissionId IS NOT NULL THEN c.ItemType ELSE NULL END AS CItemType

        //FROM Trainings t

        // LEFT JOIN TrainingsDetails d ON t.TrainingId = d.TrainingId

        //  LEFT JOIN FileMediaUplaod mf 
        //     ON mf.ReferenceId = t.TrainingId AND mf.ModuleName = @ModuleName AND mf.IsActive = 1


        //-- Training must be assigned
        //INNER JOIN Permissions p_t 
        //  ON p_t.ItemId = t.TrainingId 
        //  AND p_t.ItemType = t.ItemType 
        //  AND p_t.UserId = @UserId 
        //  AND p_t.IsAssigned = 1 
        //  AND p_t.IsActive = 1

        //-- Structure (optional, only if assigned)
        //LEFT JOIN TrainingStructure s 
        //  ON s.TrainingId = t.TrainingId

        //LEFT JOIN Permissions p_s 
        //  ON p_s.ItemId = s.TrainingStructureId 
        //  AND p_s.ItemType = s.ItemType 
        //  AND p_s.UserId = @UserId 
        //  AND p_s.IsAssigned = 1 
        //  AND p_s.IsActive = 1

        //-- Category (optional, only if assigned)
        //LEFT JOIN TrainingStructureCategory c 
        //  ON c.TrainingStructureId = s.TrainingStructureId

        //LEFT JOIN Permissions p_c 
        //  ON p_c.ItemId = c.Id
        //  AND p_c.ItemType = c.ItemType 
        //  AND p_c.UserId = @UserId 
        //  AND p_c.IsAssigned = 1 
        //  AND p_c.IsActive = 1

        //WHERE 
        //  t.TrainingId = @TrainingId
        //  AND p_t.PermissionId IS NOT NULL
        //  AND (s.TrainingStructureId IS NULL OR p_s.PermissionId IS NOT NULL)

        //ORDER BY s.TrainingStructureId, c.Id;
        //";

        //            }
        //                var trainingMap = new Dictionary<int, DtosTraining>();

        //            var result = await connection.QueryAsync<DtosTraining, DtosTrainingsDetail, DtosTrainingStructure, DtosTrainingCategory, FileMediaUplaodDto, DtosTraining>(
        //                sql,
        //                (training, detail, structure, category, file) =>
        //                {
        //                    if (!trainingMap.TryGetValue(training.TrainingId, out var dto))
        //                    {
        //                        dto = training;
        //                        trainingMap[dto.TrainingId] = dto;
        //                    }

        //                    // Add details
        //                    if (detail?.TrainingsDetailId > 0 && !dto.Details.Any(d => d.TrainingsDetailId == detail.TrainingsDetailId))
        //                        dto.Details.Add(detail);

        //                    // Add file
        //                    if (file?.FileId > 0 && !dto.FileMediaUplaod.Any(f => f.FileId == file.FileId))
        //                        dto.FileMediaUplaod.Add(file);

        //                    // Add structure and category
        //                    if (structure?.TrainingStructureId > 0)
        //                    {
        //                        var existingStructure = dto.Structures.FirstOrDefault(s => s.TrainingStructureId == structure.TrainingStructureId);
        //                        if (existingStructure == null)
        //                        {
        //                            existingStructure = structure;
        //                            dto.Structures.Add(existingStructure);
        //                        }

        //                        if (category?.Id > 0 && !existingStructure.Categories.Any(c => c.Id == category.Id))
        //                        {
        //                            existingStructure.Categories.Add(category);
        //                        }
        //                    }

        //                    return dto;
        //                },
        //                new { TrainingId = trainingId, ModuleName = ApplicationSettings.ModuleName.TrainingDetail, UserId = userId },
        //               splitOn: "TrainingsDetailId,SplitStructureId,SplitCategoryId,FileId"
        //            );

        //            var trainingDto = trainingMap.Values.FirstOrDefault();

        //            // Nest Subcategories under Parent Categories
        //            if (trainingDto != null)
        //            {
        //                foreach (var structure in trainingDto.Structures)
        //                {
        //                    var flatCategories = structure.Categories.ToList();

        //                    foreach (var cat in flatCategories.Where(c => c.SubCategoryId != null))
        //                    {
        //                        var parent = structure.Categories.FirstOrDefault(p => p.Id == cat.SubCategoryId);
        //                        if (parent != null)
        //                        {
        //                            var subCat = new DtosTrainingSubCategory
        //                            {
        //                                UserId = cat.UserId,
        //                                Type = cat.Type,
        //                                ApprovedBy = cat.ApprovedBy,
        //                                Id = cat.Id,
        //                                Status = cat.Status,
        //                                SubCategoryId = cat.SubCategoryId,
        //                                Title = cat.Title,
        //                                TrainingStructureId = cat.TrainingStructureId
        //                            };

        //                            parent.TrainingSubCategories.Add(subCat);
        //                            structure.Categories.Remove(cat);
        //                        }
        //                    }
        //                }
        //            }

        //            if (trainingDto == null)
        //                return NotFound(new { StatusCode = 404, Message = "Training not found or access denied." });

        //            return Ok(new
        //            {
        //                StatusCode = 200,
        //                Message = "Training fetched successfully.",
        //                Data = trainingDto
        //            });
        //        }
        //            catch (Exception ex)
        //            {
        //                ExceptionLogger.LogException(ex);
        //                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
        //            }
        //        }


        [HttpPost("GetTrainingWithDetails")]
        public async Task<IActionResult> GetTrainingWithDetails(int trainingId)
        {
            var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            try
            {
                using var connection = _context.CreateConnection();

                string sql = @"
        SELECT 
            t.TrainingId, t.Title AS Title, t.UserId, t.ApprovedBy, t.CreatedDate, t.Status, t.Duration,
            t.ItemType as TItemType, t.IsActive as TrainingIsActive, t.IsApproved,

            d.TrainingsDetailId, d.Title, d.Description, d.InstructorHeading,
            d.InstructorName, d.InstructorTitle, d.InstructorBio, d.Status, d.Duration,

            s.TrainingStructureId, s.Title, s.Type, s.UserId, 
            s.ApprovedBy, s.Status, s.Duration, s.TrainingId,
            s.ItemType as SItemType,

            c.Id, c.Title, c.UserId, 
            c.TrainingStructureId, c.ApprovedBy, c.Status, c.Type,c.SubCategoryId,
            c.ItemType as CItemType,

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
        LEFT JOIN TrainingStructure s ON t.TrainingId = s.TrainingId
        LEFT JOIN TrainingStructureCategory c ON s.TrainingStructureId = c.TrainingStructureId  
        LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = t.TrainingId AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
        WHERE t.TrainingId = @TrainingId AND t.IsApproved=0
        ORDER BY s.TrainingStructureId asc, c.Id asc";

                var trainingMap = new Dictionary<int, DtosTraining>();

                var result = await connection.QueryAsync<DtosTraining, DtosTrainingsDetail, DtosTrainingStructure, DtosTrainingCategory, FileMediaUplaodDto, DtosTraining>(
        sql,
        (training, detail, structure, category, file) =>
        {
            if (!trainingMap.TryGetValue(training.TrainingId, out var dto))
            {
                dto = training;
                trainingMap[dto.TrainingId] = dto;
            }

            // Map Details
            if (detail?.TrainingsDetailId > 0)
            {
                var existingDetail = dto.Details.FirstOrDefault(d => d.TrainingsDetailId == detail.TrainingsDetailId);
                if (existingDetail == null)
                {
                    existingDetail = detail;
                    dto.Details.Add(existingDetail);
                }
                if (file?.FileId > 0)
                {
                    if (!dto.FileMediaUplaod.Any(f => f.FileId == file.FileId))
                        dto.FileMediaUplaod.Add(file);
                }
            }
            if (structure?.TrainingStructureId > 0)
            {
                var existingStructure = dto.Structures.FirstOrDefault(s => s.TrainingStructureId == structure.TrainingStructureId);
                if (existingStructure == null)
                {
                    existingStructure = structure;
                    dto.Structures.Add(existingStructure);
                }
                if (category?.Id > 0 && !existingStructure.Categories.Any(c => c.Id == category.Id))
                {
                    existingStructure.Categories.Add(category);
                }
            }
            return dto;
        },
        new { TrainingId = trainingId, ModuleName = ApplicationSettings.ModuleName.TrainingDetail },
        splitOn: "TrainingsDetailId,TrainingStructureId,Id,FileId"
        );

                var trainingDto = trainingMap.Values.FirstOrDefault();

                if (trainingDto.Structures != null && trainingDto.Structures.Count > 0)
                {
                    foreach (var structure in trainingDto.Structures)
                    {
                        // Group all subcategories under their parents
                        var flatCategories = structure.Categories.ToList();

                        foreach (var cat in flatCategories.Where(c => c.SubCategoryId != null))
                        {
                            var parent = structure.Categories.FirstOrDefault(p => p.Id == cat.SubCategoryId);
                            if (parent != null)
                            {
                                var subCat = new DtosTrainingSubCategory
                                {
                                    UserId = cat.UserId,
                                    Type = cat.Type,
                                    ApprovedBy = cat.ApprovedBy,
                                    Id = cat.Id,
                                    Status = cat.Status,
                                    SubCategoryId = cat.SubCategoryId,
                                    Title = cat.Title,
                                    TrainingStructureId = cat.TrainingStructureId
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



        [HttpPost("GetTrainingDataEmployee")]
        public async Task<IActionResult> GetTrainingDataEmployee(int trainingId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("EmployeeCode");
            try
            {
                using var connection = _context.CreateConnection();

                string sql = @"
        SELECT 
            t.TrainingId, t.Title AS Title, t.UserId, t.ApprovedBy, t.CreatedDate, t.Status, t.Duration,
            t.ItemType as TItemType, t.IsActive as TrainingIsActive, t.IsApproved,

            d.TrainingsDetailId, d.Title, d.Description, d.InstructorHeading,
            d.InstructorName, d.InstructorTitle, d.InstructorBio, d.Status, d.Duration,

            s.TrainingStructureId, s.Title, s.Type, s.UserId, 
            s.ApprovedBy, s.Status, s.Duration, s.TrainingId,
            s.ItemType as SItemType,

            c.Id, c.Title, c.UserId, 
            c.TrainingStructureId, c.ApprovedBy, c.Status, c.Type,c.SubCategoryId,
            c.ItemType as CItemType,

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
        LEFT JOIN TrainingStructure s ON t.TrainingId = s.TrainingId
        LEFT JOIN TrainingStructureCategory c ON s.TrainingStructureId = c.TrainingStructureId  
        LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = t.TrainingId AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
        WHERE t.TrainingId = @TrainingId AND t.IsApproved=0 AND s.Status='1' AND c.Status='1' AND t.IsActive=1
        ORDER BY s.TrainingStructureId asc, c.Id asc";

                var trainingMap = new Dictionary<int, DtosTraining>();

                var result = await connection.QueryAsync<DtosTraining, DtosTrainingsDetail, DtosTrainingStructure, DtosTrainingCategory, FileMediaUplaodDto, DtosTraining>(
        sql,
        (training, detail, structure, category, file) =>
        {
            if (!trainingMap.TryGetValue(training.TrainingId, out var dto))
            {
                dto = training;
                trainingMap[dto.TrainingId] = dto;
            }

            // Map Details
            if (detail?.TrainingsDetailId > 0)
            {
                var existingDetail = dto.Details.FirstOrDefault(d => d.TrainingsDetailId == detail.TrainingsDetailId);
                if (existingDetail == null)
                {
                    existingDetail = detail;
                    dto.Details.Add(existingDetail);
                }
                if (file?.FileId > 0)
                {
                    if (!dto.FileMediaUplaod.Any(f => f.FileId == file.FileId))
                        dto.FileMediaUplaod.Add(file);
                }
            }
            if (structure?.TrainingStructureId > 0)
            {
                var existingStructure = dto.Structures.FirstOrDefault(s => s.TrainingStructureId == structure.TrainingStructureId);
                if (existingStructure == null)
                {
                    existingStructure = structure;
                    dto.Structures.Add(existingStructure);
                }
                if (category?.Id > 0 && !existingStructure.Categories.Any(c => c.Id == category.Id))
                {
                    existingStructure.Categories.Add(category);
                }
            }
            return dto;
        },
        new { TrainingId = trainingId, ModuleName = ApplicationSettings.ModuleName.TrainingDetail },
        splitOn: "TrainingsDetailId,TrainingStructureId,Id,FileId"
        );

                var trainingDto = trainingMap.Values.FirstOrDefault();

                if (trainingDto.Structures != null && trainingDto.Structures.Count > 0)
                {
                    foreach (var structure in trainingDto.Structures)
                    {
                        // Group all subcategories under their parents
                        var flatCategories = structure.Categories.ToList();

                        foreach (var cat in flatCategories.Where(c => c.SubCategoryId != null))
                        {
                            var parent = structure.Categories.FirstOrDefault(p => p.Id == cat.SubCategoryId);
                            if (parent != null)
                            {
                                var subCat = new DtosTrainingSubCategory
                                {
                                    UserId = cat.UserId,
                                    Type = cat.Type,
                                    ApprovedBy = cat.ApprovedBy,
                                    Id = cat.Id,
                                    Status = cat.Status,
                                    SubCategoryId = cat.SubCategoryId,
                                    Title = cat.Title,
                                    TrainingStructureId = cat.TrainingStructureId
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


        [HttpPost("GetTrainingsDetails")]
        public async Task<IActionResult> GetTrainingsDetails(TrainingsDetailFilter filters = null)
        {
            var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            try
            {


                DataTableResponce dtr = new DataTableResponce();
                var tuple = await _trainingRepository.GetTrainingsAdmin();
                dtr.iTotalRecords = tuple.Item2;
                dtr.iTotalDisplayRecords = tuple.Item2;
                dtr.aaData = tuple.Item1;
                return Ok(dtr);

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("UpsertTrainingStructureCategory")]
        public async Task<IActionResult> UpsertTrainingStructureCategory(TrainingStructureCategory category)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";

                using var connection = _context.CreateConnection();

                if (category != null)
                {
                    if (category.Id == 0 || category.Id == null) // INSERT
                    {
                        category.Id = 0;
                        const string insertQuery = @"
                        INSERT INTO TrainingStructureCategory (
                            Title,
                            UserId,
                            TrainingStructureId,
                            ApprovedBy,
                            CreatedById,
                            CreatedBy,
                            CreatedDate,
                            ModifiedById,
                            ModifiedBy,
                            ModifiedDate,
                            IsActive,
                            Status,
                            Duration,
                            TrainingId,
                            Type,
                            SubCategoryId
                        ) VALUES (
                            @Title,
                            @UserId,
                            @TrainingStructureId,
                            @ApprovedBy,
                            @CreatedById,
                            @CreatedBy,
                            @CreatedDate,
                            @ModifiedById,
                            @ModifiedBy,
                            @ModifiedDate,
                            @IsActive,
                            @Status,
                            @Duration,
                            @TrainingId,
                            @Type,
                            @SubCategoryId
                        );
                        SELECT SCOPE_IDENTITY();";

                        category.CreatedById = loggedInUserId;
                        category.CreatedBy = fullName;
                        category.CreatedDate = DateTime.UtcNow;

                        category.ModifiedById = loggedInUserId;
                        category.UserId = loggedInUserId;
                        category.ModifiedBy = fullName;
                        category.ModifiedDate = DateTime.UtcNow;
                        category.Status = ApplicationSettings.TariningStatus.Draft;

                        category.IsActive = true;

                        var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, category);

                        return Ok(new
                        {
                            StatusCode = 200,
                            Message = "Category created successfully.",
                            Data = new { Id = insertedId }
                        });
                    }
                    else // UPDATE
                    {
                        const string updateQuery = @"
                        UPDATE TrainingStructureCategory SET
                            Title = @Title,
                            UserId = @UserId,
                            TrainingStructureId = @TrainingStructureId,
                            ApprovedBy = @ApprovedBy,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            IsActive = @IsActive,
                            Status = @Status,
                            Duration = @Duration,
                            TrainingId = @TrainingId,
                            Type = @Type
                        WHERE Id = @Id";

                        category.ModifiedById = loggedInUserId;
                        category.ModifiedBy = fullName;
                        category.ModifiedDate = DateTime.UtcNow;
                        category.IsActive = true;

                        await connection.ExecuteAsync(updateQuery, category);

                        return Ok(new
                        {
                            StatusCode = 200,
                            Message = "Category updated successfully.",
                            Data = new { Id = category.Id }
                        });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("UpsertTrainingPostDetails")]
        public async Task<IActionResult> UpsertTrainingPostDetails(TrainingPostDetails model)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";

                using var connection = _context.CreateConnection();

                if (model != null)
                {
                    if (model.Id == 0) // INSERT
                    {
                        model.CreatedById = loggedInUserId;
                        model.CreatedBy = fullName;
                        model.CreatedDate = DateTime.UtcNow;
                    }
                    model.ModifiedById = loggedInUserId;
                    model.ModifiedBy = fullName;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.UserId = loggedInUserId;
                    model.IsActive = true;
                    var Id = await _trainingRepository.AddUpdateTrainingPostDetails(model);
                    if (Id > 0)
                    {
                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "Post created successfully.",
                            Data = new { Id = Id }
                        });
                    }
                    else
                    {
                        return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
                    }
                }
                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("UpsertTrainingOutline")]
        public async Task<IActionResult> UpsertTrainingOutline(TrainingOutline model)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";

                using var connection = _context.CreateConnection();

                if (model != null)
                {
                    if (model.Id == 0) // INSERT
                    {
                        model.CreatedById = loggedInUserId;
                        model.CreatedBy = fullName;
                        model.CreatedDate = DateTime.UtcNow;
                    }
                    model.ModifiedById = loggedInUserId;
                    model.ModifiedBy = fullName;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.UserId = loggedInUserId;
                    model.IsActive = true;
                    var Id = await _trainingRepository.AddUpdateTrainingOutline(model);
                    if (Id > 0)
                    {
                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = "Outline created successfully.",
                            Data = new { Id = Id }
                        });
                    }
                    else
                    {
                        return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("GetCategoryWithPosts")]
        public async Task<IActionResult> GetCategoryWithPosts(int? id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                string sql = @"
            SELECT 
                c.Id, c.Title, c.UserId, c.TrainingStructureId,
                c.ApprovedBy, c.Status, c.Type,

                p.Id AS Id, p.CategoryId, p.Description, p.CreatedBy, p.Title as Title,
                p.CreatedDate, p.ModifiedBy, p.ModifiedDate,
                p.IsActive, p.Status AS Status, p.Type AS Type,
                mf.Id AS FileId,
                mf.FileName,
                mf.FilePath,
                mf.ContentType,
                mf.Size,
                mf.ModuleName,
                mf.ReferenceId,
                mf.Extension,
                mf.IsActive AS FileIsActive,
                mf.Type AS FileType,
                mf.Title AS FileTitle,
                mf.Description AS FileDescription

            FROM HRMS.dbo.TrainingStructureCategory c
            LEFT JOIN HRMS.dbo.TrainingPostDetails p ON p.CategoryId = c.Id
            LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = c.Id AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
            WHERE c.Id = @Id;
        ";

                var categoryMap = new Dictionary<int, DtosTrainingCategoryPost>();

                var result = await connection.QueryAsync<DtosTrainingCategoryPost, DtosTrainingPost, FileMediaUplaodDto, DtosTrainingCategoryPost>(
        sql,
        (category, post, file) =>
        {
            if (!categoryMap.TryGetValue(category.Id, out var cat))
            {
                cat = category;
                cat.Posts = new List<DtosTrainingPost>();
                cat.FileMediaUplaod = new List<FileMediaUplaodDto>();
                categoryMap[cat.Id] = cat;
            }

            if (post != null && post.Id != 0)
            {
                cat.Posts.Add(post);
            }

            if (file != null && !cat.FileMediaUplaod.Any(f => f.FileId == file.FileId))
            {
                cat.FileMediaUplaod.Add(file);
            }

            return cat;
        },
                 new { Id = id, ModuleName = ApplicationSettings.ModuleName.PostDetail },
                 splitOn: "Id,FileId"
               );

                var finalResult = categoryMap.Values.FirstOrDefault();


                if (finalResult == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "Category not found." });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Category with posts fetched successfully.",
                    Data = finalResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("GetTrainingOutline")]
        public async Task<IActionResult> GetTrainingOutline(int? id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                string sql = @"
            SELECT 
                c.TrainingStructureId AS Id, c.Title, c.UserId, c.TrainingStructureId,
                c.ApprovedBy, c.Status, c.Type,

                p.Id AS Id, p.TrainingStructureId, p.Description, p.CreatedBy, p.Title as Title,
                p.CreatedDate, p.ModifiedBy, p.ModifiedDate,
                p.IsActive, p.Status AS Status, p.Type AS Type,
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

            FROM HRMS.dbo.TrainingStructure c
            LEFT JOIN HRMS.dbo.TrainingOutline p ON p.TrainingStructureId = c.TrainingStructureId
            LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = c.TrainingStructureId AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
            WHERE c.TrainingStructureId = @Id;
        ";

                var categoryMap = new Dictionary<int, TrainingStructureDto>();

                var result = await connection.QueryAsync<TrainingStructureDto, DtosTrainingOutline, FileMediaUplaodDto, TrainingStructureDto>(
        sql,
        (category, post, file) =>
        {
            if (!categoryMap.TryGetValue(category.TrainingStructureId, out var cat))
            {
                cat = category;
                cat.TrainingOutlines = new List<DtosTrainingOutline>();
                cat.FileMediaUplaod = new List<FileMediaUplaodDto>();
                categoryMap[cat.TrainingStructureId] = cat;
            }

            if (post != null && post.Id != 0)
            {
                cat.TrainingOutlines.Add(post);
            }

            if (file != null && !cat.FileMediaUplaod.Any(f => f.FileId == file.FileId))
            {
                cat.FileMediaUplaod.Add(file);
            }

            return cat;
        },
                 new { Id = id, ModuleName = ApplicationSettings.ModuleName.Outline },
                 splitOn: "Id,FileId"
               );

                var finalResult = categoryMap.Values.FirstOrDefault();


                if (finalResult == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "outline not found." });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Outline fetched successfully.",
                    Data = finalResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }




        [HttpPost("UpsertAssessmentQuestions")]
        public async Task<IActionResult> UpsertAssessmentQuestions([FromBody] List<AssessmentQuestion> questions)
        {
            if (questions == null || questions.Count == 0)
                return BadRequest(new { StatusCode = 400, Message = "Please Upload Image." });

            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";
                try
                {
                    foreach (var question in questions)
                    {

                        if (question.Id == 0)
                        {
                            question.CreatedById = loggedInUserId;
                            question.CreatedBy = fullName;
                            question.CreatedDate = DateTime.UtcNow;

                        }

                        question.ModifiedById = loggedInUserId;
                        question.ModifiedBy = fullName;
                        question.ModifiedDate = DateTime.UtcNow;
                        question.UserId ??= loggedInUserId;
                        question.IsActive = true;

                        // UPSERT question
                        var questionId = await _trainingRepository.UpsertQuestion(question);

                        // Upsert each answer
                        foreach (var answer in question.Answers)
                        {
                            // Set foreign key to question
                            answer.AssessmentQuestionId = questionId;

                            if (answer.Id == 0)
                            {
                                answer.CreatedById = loggedInUserId;
                                answer.CreatedBy = fullName;
                                answer.CreatedDate = DateTime.UtcNow;

                            }

                            answer.ModifiedById = loggedInUserId;
                            answer.ModifiedBy = fullName;
                            answer.ModifiedDate = DateTime.UtcNow;
                            answer.UserId ??= loggedInUserId;
                            answer.IsActive = true;

                            await _trainingRepository.UpsertAnswer(answer);
                        }
                    }
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Assessment questions and answers upserted successfully."
                    });
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                    return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }


        [HttpPost("UpsertAssessment")]
        public async Task<IActionResult> UpsertAssessment(Assessment model)
        {
            try
            {
                var fileMediaUpload = new FileMediaUpload();
                if (model.imageThumbnail is not null || model.imageThumbnail?.Length == 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    fileMediaUpload = await _IfileUploadService.UploadAsync(model.imageThumbnail, "images/Assessment", allowedExtensions);
                }
                if (!string.IsNullOrWhiteSpace(model?.QuestionsJson))
                {
                    try
                    {
                        model.Questions = JsonConvert.DeserializeObject<List<AssessmentQuestion>>(model.QuestionsJson);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { Message = "Invalid JSON format for questions.", Error = ex.Message });
                    }
                }
                else
                {
                    model.Questions = new List<AssessmentQuestion>();
                }
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";

                using var connection = _context.CreateConnection();

                if (model != null)
                {
                    model.ModifiedById = loggedInUserId;
                    model.ModifiedBy = fullName;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.UserId ??= loggedInUserId;
                    model.IsActive = true;

                    if (model.Id == 0 || model.Id is null) // INSERT
                    {
                        model.CreatedById = loggedInUserId;
                        model.CreatedBy = fullName;
                        model.CreatedDate = DateTime.UtcNow;
                    }
                    var id = await _trainingRepository.AddOrUpdateAssessment(model);
                    if (id > 0 && model.Questions.Count > 0)
                    {
                        foreach (var question in model.Questions)
                        {
                            // Set audit fields for question
                            if (question.Id == 0 || question.Id == null)
                            {
                                question.CreatedById = loggedInUserId;
                                question.CreatedBy = fullName;
                                question.CreatedDate = DateTime.UtcNow;

                            }
                            question.ModifiedById = loggedInUserId;
                            question.ModifiedBy = fullName;
                            question.ModifiedDate = DateTime.UtcNow;
                            question.UserId ??= loggedInUserId;
                            question.IsActive = true;

                            // UPSERT question
                            var questionId = await _trainingRepository.UpsertQuestion(question);

                            if (questionId > 0 && question.Answers.Count > 0)
                            {
                                // Upsert each answer
                                foreach (var answer in question.Answers)
                                {
                                    // Set foreign key to question
                                    answer.AssessmentQuestionId = questionId;

                                    if (answer.Id == 0 || answer.Id == null)
                                    {
                                        answer.CreatedById = loggedInUserId;
                                        answer.CreatedBy = fullName;
                                        answer.CreatedDate = DateTime.UtcNow;

                                    }
                                    answer.ModifiedById = loggedInUserId;
                                    answer.ModifiedBy = fullName;
                                    answer.ModifiedDate = DateTime.UtcNow;
                                    answer.UserId ??= loggedInUserId;
                                    answer.IsActive = true;
                                    await _trainingRepository.UpsertAnswer(answer);
                                }
                            }

                        }
                        fileMediaUpload.Id = model.ImageId;

                        if (model.imageThumbnail is not null || model.imageThumbnail?.Length == 0)
                        {
                            if (fileMediaUpload.Id == 0 || fileMediaUpload.Id == null)
                            {
                                fileMediaUpload.CreatedDate = DateTime.UtcNow;
                                fileMediaUpload.CreatedBy = fullName;
                                fileMediaUpload.CreatedById = loggedInUserId;
                                fileMediaUpload.IsActive = true;
                                fileMediaUpload.ModuleName = ApplicationSettings.ModuleName.Assessment;
                                fileMediaUpload.ReferenceId = model.CategoryId;
                                fileMediaUpload.Status = ApplicationSettings.FileMediaStatus.Active;
                            }
                            fileMediaUpload.ModifiedDate = DateTime.UtcNow;
                            fileMediaUpload.ModifiedById = loggedInUserId;
                            fileMediaUpload.ModifiedBy = fullName;
                            var isEffected = await _IfileUploadService.UpsertFileMediaAsync(fileMediaUpload);
                        }

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            Message = model.Id == 0 ? "Assessment created successfully." : "Assessment updated successfully.",
                            Data = new { Id = id }
                        });
                    }
                    else
                    {
                        return BadRequest(new { StatusCode = 400, Message = "Failed to upsert assessment." });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("GetAssessmentsByCategory")]
        public async Task<IActionResult> GetAssessmentsByCategory(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "Invalid categoryId." });
            try
            {
                var assessments = await _trainingRepository.GetAssessmentsFromFileAsync(id);

                if (assessments == null || !assessments.Any())
                    return StatusCode(200, new { StatusCode = 200, Message = "Get Assessment successfully.", Data = new AssessmentDto() });

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Message = "Get Assessment successfully.",
                    Data = assessments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpPost("UploadMedia")]
        [RequestSizeLimit(1024 * 1024 * 1024)]
        public async Task<IActionResult> UploadMedia([FromForm] UploadMediaDto model)
        {
            try
            {
                var file = new FileMediaUpload();
                if (model.File is not null || model.File?.Length == 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov", ".avi", ".mkv", ".webm", ".pdf", ".doc", ".docx" };

                    file = await _IfileUploadService.UploadAsync(model.File, model.FolderName, allowedExtensions);
                }
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";
                if (model != null)
                {
                    if (model.File is not null || model.File?.Length == 0)
                    {
                        if (model.Id == 0 || model.Id == null)
                        {
                            file.CreatedDate = DateTime.UtcNow;
                            file.CreatedBy = fullName;
                            file.CreatedById = loggedInUserId;

                        }
                        file.ModifiedDate = DateTime.UtcNow;
                        file.ModifiedById = loggedInUserId;
                        file.ModifiedBy = fullName;
                        file.IsActive = true;
                        file.ModuleName = model.ModuleName;
                        file.ReferenceId = model.ReferenceId;
                        file.Status = ApplicationSettings.FileMediaStatus.Active;
                        file.Type = model.Type;
                        file.Id = model.Id;
                        file.Description = model.Description;
                        file.Title = model.Title;
                        var isEffected = await _IfileUploadService.UpsertFileMediaAsync(file);
                        if (isEffected > 0)
                        {
                            file.Id = isEffected;
                            return StatusCode(200, new
                            {
                                StatusCode = 200,
                                Message = model.Id == 0 ? "File created successfully." : "File updated successfully.",
                                Data = file
                            });
                        }
                        else
                        {
                            return BadRequest(new { StatusCode = 400, Message = "Some thing went wrong." });
                        }
                    }
                    else
                    {
                        return BadRequest(new { StatusCode = 400, Message = "Some thing went wrong." });
                    }
                }

                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("UpsertCategoryStatus")]
        public async Task<IActionResult> UpsertCategoryStatus(int Id, string Status)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";

                using var connection = _context.CreateConnection();
                var category = new TrainingStructureCategory();

                const string updateQuery = @"
                        UPDATE TrainingStructureCategory SET
                            ApprovedBy = @ApprovedBy,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            Status = @Status
                        WHERE Id = @Id";

                category.ApprovedBy = loggedInUserId;
                category.ModifiedById = loggedInUserId;
                category.ModifiedBy = fullName;
                category.ModifiedDate = DateTime.UtcNow;
                category.Id = Id;
                category.Status = Status;

                await connection.ExecuteAsync(updateQuery, category);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Status updated successfully.",
                    Data = new { Id = category.Id }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("UpsertStructureStatus")]
        public async Task<IActionResult> UpsertStructureStatus(int Id, string Status)
        {
            try
            {
                var loggedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var fullName = $"{firstName} {lastName}";

                using var connection = _context.CreateConnection();
                var category = new TrainingStructure();

                const string updateQuery = @"
                        UPDATE TrainingStructure SET
                            ApprovedBy = @ApprovedBy,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            Status = @Status
                        WHERE TrainingStructureId = @TrainingStructureId";

                category.ApprovedBy = loggedInUserId.ToString();
                category.ModifiedById = loggedInUserId;
                category.ModifiedBy = fullName;
                category.ModifiedDate = DateTime.UtcNow;
                category.TrainingStructureId = Id;
                category.Status = Status;

                await connection.ExecuteAsync(updateQuery, category);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Status updated successfully.",
                    Data = new { Id = category.TrainingStructureId }
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
