using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.IRepository;
using HrManagement.Models;
using Humanizer;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using System.Data;
using System.Security;

namespace HrManagement.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DataContext _context;

        public PermissionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<int> UpsertPermissionAsync(TrainingPermission permission)
        {
            using var connection = _context.CreateConnection();

            var result = await GetPermissionsByRoleNameAsync(permission.ItemId, permission.PermissionId, permission.AllowedRole);


            if (permission.PermissionId == 0 || result == null)
            {
                const string insertQuery = @"
                INSERT INTO Permissions (
                    UserId, ItemType, ItemId, IsAssigned, ApprovedBy,
                    CreatedById, CreatedBy, CreatedDate,
                    ModifiedById, ModifiedBy, ModifiedDate,
                    IsActive, StartDate, ExpiryDate,AllowedRole
                ) OUTPUT INSERTED.PermissionId
                VALUES (
                    @UserId, @ItemType, @ItemId, @IsAssigned, @ApprovedBy,
                    @CreatedById, @CreatedBy, @CreatedDate,
                    @ModifiedById, @ModifiedBy, @ModifiedDate,
                    @IsActive, @StartDate, @ExpiryDate, @AllowedRole
                );";

                return await connection.ExecuteScalarAsync<int>(insertQuery, permission);
            }
            else
            {
                const string updateQuery = @"
                UPDATE Permissions SET
                    UserId = @UserId,
                    ItemType = @ItemType,
                    ItemId = @ItemId,
                    IsAssigned = @IsAssigned,
                    ApprovedBy = @ApprovedBy,
                    ModifiedById = @ModifiedById,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive,
                    StartDate = @StartDate,
                    ExpiryDate = @ExpiryDate,
                    AllowedRole=@AllowedRole
                WHERE PermissionId = @PermissionId";

                await connection.ExecuteScalarAsync(updateQuery, permission);
                return permission.PermissionId;
            }
        }

        public async Task<DtosTraining> GetTrainingWithPermissions(int userId, DtosTraining training)
        {

            var Permissions = await GetPermissionsByUserIdAsync(userId);

            bool trainingAssigned = Permissions.Any(p =>
                p.ItemType == 1 && p.ItemId == training.TrainingId && p.IsAssigned == true);

            if (!trainingAssigned || training.TrainingIsActive == false)
                return null;

            training.Structures = training.Structures
                .Where(s =>
                    s.Status == "1" &&
                     Permissions.Any(p =>
                         p.ItemType == 2 && p.ItemId == s.TrainingStructureId && p.IsAssigned == true))
                .ToList();

            foreach (var structure in training.Structures)
            {
                structure.Categories = structure.Categories
                    .Where(c =>
                        c.Status == "1" &&
                        Permissions.Any(p =>
                             p.ItemType == 3 && p.ItemId == c.Id && p.IsAssigned == true))
                    .ToList();

                foreach (var cat in structure.Categories)
                {
                    cat.TrainingSubCategories = cat.TrainingSubCategories
                        .Where(sc =>
                            sc.Status == "1" &&
                            Permissions.Any(p =>
                                 p.ItemType == 3 && p.ItemId == cat.Id && p.IsAssigned == true))
                        .ToList();
                }
            }
            return training;
        }

        public async Task<List<TrainingCombinedModel>> TrainingDeshBoard(int userId, List<TrainingCombinedModel> trainings, string UserRoleName)
        {
            var Permissions = await GetPermissionsByUserIdAsync(userId);
            var item = trainings
                     .Where(c =>
                         c.IsActive == true &&
                         Permissions.Any(p =>
                              p.ItemType == 1 && p.ItemId == c.TrainingId && p.IsAssigned == true))
                     .ToList();
            return item;
        }

        public async Task<TrainingLessonSession> WatchLessonPermissionsUser(int userId, TrainingLessonSession training)
        {

            var Permissions = await GetPermissionsByUserIdAsync(userId);

            bool trainingAssigned = Permissions.Any(p =>
                p.ItemType == 1 && p.ItemId == training.TrainingId && p.IsAssigned == true);

            if (!trainingAssigned || training.TrainingIsActive == false)
                return null;

            training.Structures = training.Structures
                .Where(s =>
                    s.SStatus == "1" &&
                     Permissions.Any(p =>
                         p.ItemType == 2 && p.ItemId == s.TrainingStructureId && p.IsAssigned == true))
                .ToList();

            foreach (var structure in training.Structures)
            {
                structure.Categories = structure.Categories
                    .Where(c =>
                        c.CStatus == "1" &&
                        Permissions.Any(p =>
                             p.ItemType == 3 && p.ItemId == c.CategoryId && p.IsAssigned == true))
                    .ToList();

                foreach (var cat in structure.Categories)
                {
                    cat.TrainingSubCategories = cat.TrainingSubCategories
                        .Where(sc =>
                            sc.CStatus == "1" &&
                            Permissions.Any(p =>
                                 p.ItemType == 3 && p.ItemId == sc.CategoryId && p.IsAssigned == true))
                        .ToList();
                }
            }
            return training;
        }
        public async Task<IEnumerable<TrainingPermission>> GetPermissionsByUserIdAsync(int userId)
        {
            using var connection = _context.CreateConnection();

            const string query = @"
                   SELECT PermissionId, UserId, ItemType, ItemId, IsAssigned, ApprovedBy,
                   CreatedById, CreatedBy, CreatedDate,
                   ModifiedById, ModifiedBy, ModifiedDate,
                   IsActive, StartDate, ExpiryDate
                   FROM Permissions
                   WHERE UserId = @UserId AND IsActive = 1";

            var result = await connection.QueryAsync<TrainingPermission>(query, new { UserId = userId });
            return result;
        }

        public async Task<TrainingPermission> GetPermissionsByRoleNameAsync(int? ItemId, int PermissionId, string RoleName)
        {
            using var connection = _context.CreateConnection();
            const string query = @"
                   SELECT PermissionId, UserId, ItemType, ItemId, IsAssigned, ApprovedBy,
                   CreatedById, CreatedBy, CreatedDate,
                   ModifiedById, ModifiedBy, ModifiedDate,
                   IsActive, StartDate, ExpiryDate
                   FROM Permissions
                   WHERE IsActive = 1 AND ItemId=@ItemId AND PermissionId=@PermissionId AND AllowedRole=@RoleName";

            var result = await connection.QueryFirstOrDefaultAsync<TrainingPermission>(query, new { ItemId = ItemId, PermissionId = PermissionId, RoleName = RoleName });
            return result;
        }

        public async Task<List<TrainingPermissionDto>> GetPermissionRole(string RoleName)
        {
            using var connection = _context.CreateConnection();

            var query = @"
        SELECT 
            t.TrainingId, 
            t.Title, 
            t.ItemType, 
            p.PermissionId, 
            p.IsAssigned, 
            p.AllowedRole
        FROM Trainings t
        LEFT JOIN Permissions p 
            ON p.ItemId = t.TrainingId 
            AND p.ItemType = '1' 
            AND p.IsActive = 1 
            AND p.AllowedRole = @RoleName
        WHERE t.IsActive = 1 
          AND t.IsApproved = 0";

            var result = await connection.QueryAsync<TrainingPermissionDto>(query, new { RoleName });
            return result.ToList();
        }
    }
}
