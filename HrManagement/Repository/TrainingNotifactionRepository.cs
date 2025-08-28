using Dapper;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.IRepository;
using HrManagement.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;


namespace HrManagement.Repository
{
    public class TrainingNotifactionRepository : ITrainingNotifactionRepository
    {
        private readonly Email _email;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        public TrainingNotifactionRepository(Email email, DataContext context, IWebHostEnvironment env)
        {
            _email = email;

            _context = context;
            _env = env;

        }
        public List<string> GetEmployeeCodesList(string EmployeeCodes)
        {
            return ParseJsonStringToList(EmployeeCodes);
        }
        public List<string> GetDepartmentIdsList(string DepartmentIds)
        {
            return ParseJsonStringToList(DepartmentIds);
        }
        public List<string> GetSiteIdsList(string SiteIds)
        {
            return ParseJsonStringToList(SiteIds);
        }
        private List<string> ParseJsonStringToList(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
            }
            catch (JsonException)
            {
                return jsonString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .ToList();
            }
        }
        public async Task<bool> SendEmail(TrainingAssignModel assignTraining, int maxRetries = 3, int batchSize = 5)
        {
            try
            {
                var employees = await GetEmployeesBasedOnVisibility(assignTraining);

                if (!employees.Any())
                {
                    return false;
                }
                var results = new List<bool>();
                var employeeList = employees.ToList();

                for (int i = 0; i < employeeList.Count; i += batchSize)
                {
                    var batch = employeeList.Skip(i).Take(batchSize);
                    var batchTasks = batch.Select(employee =>
                        SendEmailWithRetry(employee, assignTraining, maxRetries)).ToArray();

                    var batchResults = await Task.WhenAll(batchTasks);
                    results.AddRange(batchResults);

                    if (i + batchSize < employeeList.Count)
                    {
                        await Task.Delay(1000); // 1 second delay
                    }
                }
                foreach (var employee in employees)
                {
                    var sendNotifcation = await SendTrainingNotifacation(employee);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<IEnumerable<EmployeeTrainingEmail>> GetEmployeesBasedOnVisibility(TrainingAssignModel assignTraining)
        {
            string query;
            object parameters;

            switch (assignTraining.VisibleTo)
            {
                case "Departments":
                    var departmentIds = GetDepartmentIdsList(assignTraining.Departments);
                    if (!departmentIds.Any())
                    {
                        return Enumerable.Empty<EmployeeTrainingEmail>();
                    }

                    query = @"
                           SELECT DISTINCT e.EmployeeCode, e.FirstName, e.LastName, e.Email
                           FROM EMPLOYEE e
                           INNER JOIN DEPARTMENT d ON e.DepartmentId = d.DepartmentId
                           WHERE d.DepartmentId IN @DepartmentIds
                           AND e.Email IS NOT NULL
                           AND e.Email != ''";
                    parameters = new { DepartmentIds = departmentIds };
                    break;

                case "Sites":
                    var siteIds = GetDepartmentIdsList(assignTraining.Departments);
                    if (!siteIds.Any())
                    {
                        return Enumerable.Empty<EmployeeTrainingEmail>();
                    }
                    query = @"SELECT DISTINCT e.EmployeeCode, e.FirstName, e.LastName, e.Email
                  FROM EMPLOYEE e
                  INNER JOIN SITES s ON e.SiteId = s.Id
                  WHERE s.Id IN @SiteIds
                  AND e.Email IS NOT NULL
                  AND e.Email != ''";
                    parameters = new { SiteIds = siteIds };
                    break;

                case "employees":
                    var employeeCodes = GetEmployeeCodesList(assignTraining.Employees);
                    if (!employeeCodes.Any())
                    {
                        return Enumerable.Empty<EmployeeTrainingEmail>();
                    }

                    query = @"SELECT EmployeeCode, FirstName, LastName, Email
                  FROM EMPLOYEE
                  WHERE EmployeeCode IN @EmployeeCodes
                  AND Email IS NOT NULL
                  AND Email != ''";
                    parameters = new { EmployeeCodes = employeeCodes };
                    break;

                default:

                    return Enumerable.Empty<EmployeeTrainingEmail>();
            }
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<EmployeeTrainingEmail>(query, parameters);
        }
        private async Task<bool> SendEmailWithRetry(EmployeeTrainingEmail employee, TrainingAssignModel assignTraining, int maxRetries)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await SendTrainingAssignmentEmail(employee, assignTraining);
                    return true;
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        return false;
                    }
                    var delayMs = (int)(Math.Pow(2, attempt) * 1000);
                    await Task.Delay(delayMs);
                }
            }
            return true;
        }
        private async Task SendTrainingAssignmentEmail(EmployeeTrainingEmail employee, TrainingAssignModel assignTraining)
        {
            try
            {
                string imagePath = Path.Combine(_env.WebRootPath, "images", "Revital-Health-2.png");
                string contentId = "logoImage";
                string Date = DateTime.Now.ToString("yyyy-MM-dd");
                Console.WriteLine(Date);
                if (employee != null)
                {
                    var subject = $"Training Invitation: {assignTraining.title}";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                            <p>Dear {employee.FirstName} {employee.LastName},</p>

                            <p>
                                You have been assigned a new training titled: <strong>{assignTraining.title}</strong>.
                            </p>
                            <p>
                                <strong>Status:</strong> Pending
                            </p>
                            <p>
                                Please log in to the HRMS system to view the training details and complete it within the assigned timeframe.
                            </p>
                            <p>
                                <a href='https://hrms.chestermerephysio.ca/' target='_blank' style='color: #007BFF; text-decoration: none;'>
                                    Access HRMS Training Portal
                                </a>
                            </p>

                            <p>
                                Timely completion of training is important for your professional development and is required as part of company policy.
                            </p>

                            <p>
                                If you have any questions regarding this training, please contact your manager or the HR department.
                            </p>
                            <p style='margin-top: 30px;'>
                                Best regards,<br/>
                                <strong>HRMS Team</strong><br/>
                                Revital Health
                            </p>
                            <img src='cid:{contentId}' alt='Revital Health Logo' style='height:60px; margin-top:10px;' />
                        </div>";

                    LinkedResource logo = new LinkedResource(imagePath, MediaTypeNames.Image.Png)
                    {
                        ContentId = contentId,
                        TransferEncoding = TransferEncoding.Base64
                    };

                    await _email.SendEmailAsync(
                    from: "info@hrms.chestermerephysio.ca",
                    to: employee.Email,
                    toname: employee.FirstName + " " + employee.LastName,
                    cc: null,
                    bcc: null,
                    subject: subject,
                    body: body,
                    AttachmentsPaths: null,
                    IsBodyHtml: true,
                    delimitter: ';'
                );
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }
        public async Task<int> SendTrainingNotifacation(EmployeeTrainingEmail employee)
        {
            using var connection = _context.CreateConnection();
            var NotificationQuery = @"INSERT INTO Notifications
                                                  (
                                                     NotificationType
                                                    ,NotificationDescription
                                                    ,NotificationUrl
                                                    ,EmployeeCode
                                                    ,CreatedDate
                                                    ,IsActive
                                                  )
                                                  VALUES
                                                  (
                                                     @NotificationType
                                                    ,@NotificationDescription
                                                    ,@NotificationUrl
                                                    ,@EmployeeCode
                                                    ,@CreatedDate
                                                    ,@IsActive
                                                  );";

            var notif = new Notifications();
            notif.EmployeeCode = employee.EmployeeCode;
            notif.NotificationType = "New Training";
            notif.NotificationDescription = "New training course has been assigned to you.";
            notif.NotificationUrl = "Products/Index";
            notif.CreatedDate = DateTime.UtcNow;
            notif.IsActive = true;
            var isEffected = await connection.ExecuteAsync(NotificationQuery, notif);
            return isEffected;
        }
    }
}
