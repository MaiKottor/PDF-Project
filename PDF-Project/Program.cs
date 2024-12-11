var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddControllersWithViews(); // MVC services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
else
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable static file serving (CSS/JS if used)
app.UseRouting(); // Must come before authorization
app.UseAuthorization();

// Configure routing
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"); // Default route

app.Run();

