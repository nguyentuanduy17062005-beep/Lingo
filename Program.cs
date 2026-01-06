using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LingoAppNet8.Data;
using LingoAppNet8.Forms;

namespace LingoAppNet8;

static class Program
{
    public static ServiceProvider? ServiceProvider { get; private set; }

    [STAThread]
    static void Main()
    {
        try
        {
            File.WriteAllText("startup_log.txt", "Starting application...\n");
            
            // Setup Dependency Injection
            File.AppendAllText("startup_log.txt", "Configuring services...\n");
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            File.AppendAllText("startup_log.txt", "Services configured.\n");

            // Initialize database
            File.AppendAllText("startup_log.txt", "Creating database...\n");
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LingoDbContext>();
                context.Database.EnsureCreated();
            }
            File.AppendAllText("startup_log.txt", "Database created.\n");

            File.AppendAllText("startup_log.txt", "Initializing application...\n");
            ApplicationConfiguration.Initialize();
            
            File.AppendAllText("startup_log.txt", "Creating LoginForm...\n");
            var loginForm = new LoginForm();
            
            File.AppendAllText("startup_log.txt", "Running application...\n");
            Application.Run(loginForm);
            
            File.AppendAllText("startup_log.txt", "Application closed normally.\n");
        }
        catch (Exception ex)
        {
            var errorMsg = $"ERROR at {DateTime.Now}:\n{ex.GetType().Name}: {ex.Message}\n\nStack trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}\n";
            File.WriteAllText("error_log.txt", errorMsg);
            MessageBox.Show($"Lỗi khởi động ứng dụng. Xem file error_log.txt để biết chi tiết.\n\n{ex.Message}", 
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<LingoDbContext>(options =>
            options.UseSqlServer("Server=LAPTOP-7TOIFEJI\\SQLEXPRESS;Database=LingoDb;Integrated Security=True;TrustServerCertificate=True;"));
    }
}