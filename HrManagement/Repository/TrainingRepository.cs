using Dapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HrManagement.ApplicationSetting;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.IRepository;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;


namespace HrManagement.Repository
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly DataContext _context;
        private readonly IPermissionRepository _permissionRepository;


        public TrainingRepository(DataContext context, IPermissionRepository permissionRepository)
        {
            _context = context;
            _permissionRepository = permissionRepository;
        }


        public async Task<bool> UpsertTrainingStructure(TrainingStructure trainingStructure)
        {

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



                    var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, trainingStructure);
                    if (insertedId > 0)
                    {
                        return true;
                    }
                    return false;

                }
                else // Update
                {
                    const string updateQuery = @"
                                                  UPDATE TrainingStructure SET
                                                      Title = @Title,
                                                      Type=@Type
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

                    var insertedId = await connection.ExecuteAsync(updateQuery, trainingStructure);

                    if (insertedId > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> UpsertTrainingDetail(TrainingsDetail trainingsDetail)
        {

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



                    var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, trainingsDetail);
                    if (insertedId > 0)
                    {
                        return true;
                    }
                    return false;

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

                    var insertedId = await connection.ExecuteAsync(updateQuery, trainingsDetail);

                    if (insertedId > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public async Task<(List<TrainingCombinedModel> Data, int TotalCount)> GetTrainingsCombinedAsync(int UserId, string UserRoleName)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            SELECT
            t.TrainingId,
            t.ItemType,
            t.Url,
            t.IsExternal,
            t.ApprovedBy,
            t.IsActive,
            td.TrainingsDetailId,
            td.Title,
            td.Description,
            td.ProductThumbnail,
            td.InstructorHeading,
            td.Headshot,
            td.InstructorName,
            td.InstructorTitle,
            td.InstructorBio,
            td.LogoImage,
            td.CreatedById,
            td.Amount,
            td.PaymentType,
            td.CreatedBy,
            td.CreatedDate,
            td.ModifiedById,
            td.ModifiedBy,
            td.ModifiedDate,
            td.Status,
            td.Duration,
            td.UserId,
            mf.FilePath
           ,t.VisibleTo
           ,t.Departments
           ,t.DepartmentsSubCategories
           ,t.Employees
           ,t.Sites
        FROM [HRMS].[dbo].[Trainings] t
        LEFT JOIN [HRMS].[dbo].[TrainingsDetails] td ON t.TrainingId = td.TrainingId
        LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = t.TrainingId AND mf.ModuleName = '3' AND mf.Type='TrainingThumbnail' AND mf.IsActive = 1
        WHERE t.IsActive = 1
        ORDER BY t.TrainingId DESC
    ";

            var data = (await connection.QueryAsync<TrainingCombinedModel>(sql)).ToList();

            var item = await _permissionRepository.TrainingDeshBoard(UserId, data, UserRoleName);
            int totalCount = item.Count;
            return (item, totalCount);
        }

        public async Task<(List<TrainingCombinedModel> Data, int TotalCount)> GetTrainingsAdmin()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
            SELECT
            t.TrainingId,
            t.IsApproved,
            t.ItemType,
            t.Url,
            t.IsExternal,
            t.ApprovedBy,
            t.IsActive,
            td.TrainingsDetailId,
            td.Title,
            td.Amount,
            td.PaymentType,
            td.Description,
            td.ProductThumbnail,
            td.InstructorHeading,
            td.Headshot,
            td.InstructorName,
            td.InstructorTitle,
            td.InstructorBio,
            td.LogoImage,
            td.CreatedById,
            td.CreatedBy,
            td.CreatedDate,
            td.ModifiedById,
            td.ModifiedBy,
            td.ModifiedDate,
            td.Status,
            td.Duration,
            td.UserId,
            mf.FilePath
           ,t.VisibleTo
           ,t.Departments
           ,t.DepartmentsSubCategories
           ,t.Employees
           ,t.Sites
        FROM [HRMS].[dbo].[Trainings] t
        LEFT JOIN [HRMS].[dbo].[TrainingsDetails] td ON t.TrainingId = td.TrainingId
         LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = t.TrainingId AND mf.ModuleName = '3' AND mf.Type='TrainingThumbnail' AND mf.IsActive = 1
        Where t.IsApproved=0
        ORDER BY t.TrainingId DESC
    ";

            var data = (await connection.QueryAsync<TrainingCombinedModel>(sql)).ToList();

            int totalCount = data.Count;
            return (data, totalCount);
        }


        public async Task<int> AddUpdateTrainingPostDetails(TrainingPostDetails model)
        {

            using var connection = _context.CreateConnection();
            if (model.Id == 0) // INSERT
            {

                const string insertQuery = @"
                INSERT INTO TrainingPostDetails (
                Title,
                UserId,
                CategoryId,
                ApprovedBy,
                CreatedById,
                CreatedBy,
                CreatedDate,
                ModifiedById,
                ModifiedBy,
                ModifiedDate,
                IsActive,
                Status,
                Description,
                Type
              
            )
            OUTPUT INSERTED.Id
            VALUES (
                @Title,
                @UserId,
                @CategoryId,
                @ApprovedBy,
                @CreatedById,
                @CreatedBy,
                @CreatedDate,
                @ModifiedById,
                @ModifiedBy,
                @ModifiedDate,
                @IsActive,
                @Status,
                @Description,
                @Type
               
            );";

                var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, model);
                if (insertedId > 0)
                {
                    const string updateStructureQuery = @"
                        UPDATE TrainingStructureCategory SET
                            Title = @Title
                            WHERE Id = @Id";

                    var StructureCategory = new TrainingStructureCategory();

                    StructureCategory.ModifiedById = model.ModifiedById;
                    StructureCategory.Title = model.Title;
                    StructureCategory.ModifiedBy = model.ModifiedBy;
                    StructureCategory.ModifiedDate = DateTime.UtcNow;
                    StructureCategory.IsActive = true;
                    StructureCategory.Id = (int)model.CategoryId;
                    await connection.ExecuteAsync(updateStructureQuery, StructureCategory);

                    return insertedId;
                }
                else { return 0; }
            }
            else
            {
                const string updateQuery = @"
                        UPDATE TrainingPostDetails SET
                            Title = @Title,
                            UserId = @UserId,
                            CategoryId = @CategoryId,
                            ApprovedBy = @ApprovedBy,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            IsActive = @IsActive,
                            Status = @Status,
                            Description = @Description
                           
                            Type = @Type
                            WHERE Id = @Id";
                int UpdatedId = await connection.ExecuteAsync(updateQuery, model);
                if (model.Id > 0)
                {
                    const string updateStructureQuery = @"
                        UPDATE TrainingStructureCategory SET
                            Title = @Title
                            WHERE Id = @Id";

                    var StructureCategory = new TrainingStructureCategory();

                    StructureCategory.ModifiedById = model.ModifiedById;
                    StructureCategory.Title = model.Title;
                    StructureCategory.ModifiedBy = model.ModifiedBy;
                    StructureCategory.ModifiedDate = DateTime.UtcNow;
                    StructureCategory.IsActive = true;
                    StructureCategory.Id = (int)model.CategoryId;
                    await connection.ExecuteAsync(updateStructureQuery, StructureCategory);
                    return model.Id;
                }
                else { return 0; }
            }
        }


        public async Task<int> AddUpdateTrainingOutline(TrainingOutline model)
        {

            using var connection = _context.CreateConnection();
            if (model.Id == 0) // INSERT
            {
                const string insertQuery = @"
                INSERT INTO TrainingOutline (
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
                Description,
                Type
            )
            OUTPUT INSERTED.Id
            VALUES (
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
                @Description,
                @Type
            );";
                var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, model);
                if (insertedId > 0)
                {
                    const string updateStructureQuery = @"
                        UPDATE TrainingStructure SET
                            Title = @Title
                            WHERE TrainingStructureId = @TrainingStructureId";

                    var trainingStructure = new TrainingStructure();

                    trainingStructure.ModifiedById = model.ModifiedById;
                    trainingStructure.Title = model.Title;
                    trainingStructure.ModifiedBy = model.ModifiedBy;
                    trainingStructure.ModifiedDate = DateTime.UtcNow;
                    trainingStructure.IsActive = true;
                    trainingStructure.TrainingStructureId = (int)model.TrainingStructureId;
                    await connection.ExecuteAsync(updateStructureQuery, trainingStructure);
                    return insertedId;
                }
                else { return 0; }
            }
            else
            {
                const string updateQuery = @"
                        UPDATE TrainingOutline SET
                            Title = @Title,
                            UserId = @UserId,
                            TrainingStructureId = @TrainingStructureId,
                            ApprovedBy = @ApprovedBy,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            IsActive = @IsActive,
                            Status = @Status,
                            Description = @Description,
                            Type = @Type
                            WHERE Id = @Id";
                int UpdatedId = await connection.ExecuteAsync(updateQuery, model);
                if (model.Id > 0)
                {
                    const string updateStructureQuery = @"
                        UPDATE TrainingStructure SET
                            Title = @Title
                            WHERE TrainingStructureId = @TrainingStructureId";

                    var trainingStructure = new TrainingStructure();

                    trainingStructure.ModifiedById = model.ModifiedById;
                    trainingStructure.Title = model.Title;
                    trainingStructure.ModifiedBy = model.ModifiedBy;
                    trainingStructure.ModifiedDate = DateTime.UtcNow;
                    trainingStructure.IsActive = true;
                    trainingStructure.TrainingStructureId = (int)model.TrainingStructureId;
                    await connection.ExecuteAsync(updateStructureQuery, trainingStructure);
                    return model.Id;
                }
                else { return 0; }
            }
        }


        public async Task<int> UpsertQuestion(AssessmentQuestion question)
        {
            using var connection = _context.CreateConnection();
            if (question.Id == 0)
            {
                // Insert
                var insertSql = @"
            INSERT INTO [HRMS].[dbo].[AssessmentQuestion]
            (Title, UserId, CategoryId, ApprovedBy, CreatedById, CreatedBy, CreatedDate, ModifiedById, ModifiedBy, ModifiedDate, IsActive, Status, Description, Type)
            VALUES
            (@Title, @UserId, @CategoryId, @ApprovedBy, @CreatedById, @CreatedBy, @CreatedDate, @ModifiedById, @ModifiedBy, @ModifiedDate, @IsActive, @Status, @Description, @Type);
            SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await connection.QuerySingleAsync<int>(insertSql, question);
                return id;
            }
            else
            {
                // Update
                var updateSql = @"
            UPDATE [HRMS].[dbo].[AssessmentQuestion]
            SET Title = @Title,
                UserId = @UserId,
                CategoryId = @CategoryId,
                ApprovedBy = @ApprovedBy,
                ModifiedById = @ModifiedById,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = @ModifiedDate,
                IsActive = @IsActive,
                Status = @Status,
                Description = @Description,
                Type = @Type
            WHERE Id = @Id";

                await connection.ExecuteAsync(updateSql, question);
                return question.Id;
            }
        }

        public async Task UpsertAnswer(AssessmentAnswer answer)
        {
            using var connection = _context.CreateConnection();
            if (answer.Id == 0)
            {
                var insertSql = @"
            INSERT INTO [HRMS].[dbo].[AssessmentAnswer]
            (Title, IsCorrect, UserId, CategoryId, AssessmentQuestionId, ApprovedBy, CreatedById, CreatedBy, CreatedDate, ModifiedById, ModifiedBy, ModifiedDate, IsActive, Status, Description, Type)
            VALUES
            (@Title, @IsCorrect, @UserId, @CategoryId, @AssessmentQuestionId, @ApprovedBy, @CreatedById, @CreatedBy, @CreatedDate, @ModifiedById, @ModifiedBy, @ModifiedDate, @IsActive, @Status, @Description, @Type);";

                await connection.ExecuteAsync(insertSql, answer);
            }
            else
            {
                var updateSql = @"
            UPDATE [HRMS].[dbo].[AssessmentAnswer]
            SET Title = @Title,
                IsCorrect = @IsCorrect,
                UserId = @UserId,
                CategoryId = @CategoryId,
                ApprovedBy = @ApprovedBy,
                ModifiedById = @ModifiedById,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = @ModifiedDate,
                IsActive = @IsActive,
                Status = @Status,
                Description = @Description,
                Type = @Type
            WHERE Id = @Id";

                await connection.ExecuteAsync(updateSql, answer);
            }
        }


        public async Task<int> AddOrUpdateAssessment(Assessment model)
        {
            var query = model.Id == 0 || model.Id is null
                ? @"
            INSERT INTO [HRMS].[dbo].[Assessment]
            (
                PassingGrade,
                Title,
                IsPassingGrade,
                UserId,
                CategoryId,
                ApprovedBy,
                CreatedById,
                CreatedBy,
                CreatedDate,
                ModifiedById,
                ModifiedBy,
                ModifiedDate,
                IsActive,
                Status,
                PassConfirmationMessage,
                FailConfirmationMessage
            )
            VALUES
            (
                @PassingGrade,
                @Title,
                @IsPassingGrade,
                @UserId,
                @CategoryId,
                @ApprovedBy,
                @CreatedById,
                @CreatedBy,
                @CreatedDate,
                @ModifiedById,
                @ModifiedBy,
                @ModifiedDate,
                @IsActive,
                @Status,
                @PassConfirmationMessage,
                @FailConfirmationMessage
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);"
                : @"
            UPDATE [HRMS].[dbo].[Assessment]
            SET
                PassingGrade = @PassingGrade,
                Title = @Title,
                IsPassingGrade = @IsPassingGrade,
                UserId = @UserId,
                CategoryId = @CategoryId,
                ApprovedBy = @ApprovedBy,
                ModifiedById = @ModifiedById,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = @ModifiedDate,
                IsActive = @IsActive,
                Status = @Status,
                PassConfirmationMessage = @PassConfirmationMessage,
                FailConfirmationMessage = @FailConfirmationMessage
            WHERE Id = @Id;
            SELECT @Id;";


            const string updateStructureQuery = @"
                        UPDATE TrainingStructureCategory SET
                            Title = @Title
                            WHERE Id = @Id";

            var StructureCategory = new TrainingStructureCategory();

            StructureCategory.ModifiedById = model.ModifiedById;
            StructureCategory.Title = model.title;
            StructureCategory.ModifiedBy = model.ModifiedBy;
            StructureCategory.ModifiedDate = DateTime.UtcNow;
            StructureCategory.IsActive = true;
            StructureCategory.Id = (int)model.CategoryId;
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(updateStructureQuery, StructureCategory);


            return await connection.ExecuteScalarAsync<int>(query, model);
        }


        public async Task<List<AssessmentDto>> GetAssessmentsFromFileAsync(int categoryId)
        {
            string sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", "Assessment", "EditAssessment.sql");
            string sql = await File.ReadAllTextAsync(sqlFilePath);
            var assessmentDict = new Dictionary<int, AssessmentDto>();
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<AssessmentDto, AssessmentQuestionDto, AssessmentAnswerDto, FileMediaUplaodDto, AssessmentDto>(
                sql,
                (assessment, question, answer, file) =>
                {
                    if (!assessmentDict.TryGetValue(assessment.AssessmentId, out var currentAssessment))
                    {
                        currentAssessment = assessment;
                        currentAssessment.Questions = new List<AssessmentQuestionDto>();
                        currentAssessment.FileMediaUplaod = new List<FileMediaUplaodDto>();
                        assessmentDict.Add(currentAssessment.AssessmentId, currentAssessment);
                    }

                    if (question != null && !currentAssessment.Questions.Any(q => q.QuestionId == question.QuestionId))
                    {
                        question.Answers = new List<AssessmentAnswerDto>();
                        currentAssessment.Questions.Add(question);
                    }

                    var currentQuestion = currentAssessment.Questions.FirstOrDefault(q => q.QuestionId == question?.QuestionId);
                    if (currentQuestion != null && answer != null && !currentQuestion.Answers.Any(a => a.AnswerId == answer.AnswerId))
                    {
                        currentQuestion.Answers.Add(answer);
                    }

                    if (file != null && !currentAssessment.FileMediaUplaod.Any(f => f.FileId == file.FileId))
                    {
                        currentAssessment.FileMediaUplaod.Add(file);
                    }

                    return currentAssessment;
                },
                param: new { Id = categoryId, ModuleName = ApplicationSettings.ModuleName.Assessment },
                splitOn: "QuestionId,AnswerId,FileId"
            );

            return assessmentDict.Values.ToList();
        }

        public async Task<int> CloneFullTrainingAsync(int trainingId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@TrainingId", trainingId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@NewTrainingId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("CloneFullTraining", parameters, commandType: CommandType.StoredProcedure);

            // Get the output parameter
            int newTrainingId = parameters.Get<int>("@NewTrainingId");

            return newTrainingId;
        }

    }
}

