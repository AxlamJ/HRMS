using Dapper;
using DocumentFormat.OpenXml.InkML;
using HrManagement.Data;
using HrManagement.IRepository;
using HrManagement.Models;
using System.Data;

namespace HrManagement.Repository
{
    public class TrainingCommentRepository : ITrainingCommentRepository
    {
        private readonly DataContext _context;

        public TrainingCommentRepository(DataContext connection)
        {
            _context = connection;
        }

        public async Task<IEnumerable<TrainingComment>> GetCommentsByCategoryAsync(int categoryId, int UserId)
        {
            using var _connection = _context.CreateConnection();
            var sql = "SELECT * FROM TrainingComments WHERE CategoryId = @CategoryId AND IsActive = 1";
            return await _connection.QueryAsync<TrainingComment>(sql, new { CategoryId = categoryId });
        }

        public async Task<TrainingComment> GetCommentByIdAsync(int commentId)
        {
            using var _connection = _context.CreateConnection();
            var sql = "SELECT * FROM TrainingComments WHERE CommentId = @CommentId AND IsActive = 1";
            return await _connection.QueryFirstOrDefaultAsync<TrainingComment>(sql, new { CommentId = commentId });
        }

        public async Task<int> AddCommentAsync(TrainingComment comment)
        {
            using var _connection = _context.CreateConnection();
            var sql = @"
            INSERT INTO TrainingComments (CategoryId, UserId, CommentText, Status, CreatedAt, IsActive, CreatedBy)
            VALUES (@CategoryId, @UserId, @CommentText, @Status, GETDATE(), 1, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _connection.ExecuteScalarAsync<int>(sql, comment);
        }

        public async Task<bool> UpdateCommentAsync(TrainingComment comment)
        {
            using var _connection = _context.CreateConnection();
            var sql = @"
            UPDATE TrainingComments
            SET CommentText = @CommentText,
                Status = @Status,
                UpdatedAt = GETDATE()
            WHERE CommentId = @CommentId AND IsActive = 1";

            var result = await _connection.ExecuteAsync(sql, comment);
            return result > 0;
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            using var _connection = _context.CreateConnection();
            var sql = @"
            UPDATE TrainingComments
            SET IsActive = 0, UpdatedAt = GETDATE()
            WHERE CommentId = @CommentId";

            var result = await _connection.ExecuteAsync(sql, new { CommentId = commentId });
            return result > 0;
        }
    }

}
