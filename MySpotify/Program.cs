using Microsoft.EntityFrameworkCore;
using MySpotify.Models;
using MySpotify.Repository;

/*
при проверки логина-пароля проверять активирован ли юзер. 
добавить событие сохранения юзера.




разрешить пользователю удалять песни из плейлиста.
 
*/




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MediaUserContext>(options => options.UseSqlServer(connection));


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepositoryUser, UserRepository>();
builder.Services.AddTransient<IRepositoryMedia, MediaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();


/*не сохраняет в БД медиа!*/




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.Run();



