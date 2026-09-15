using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GymFit.Web.Controllers;
[Authorize]
public class NotificationsController:Controller{
 private readonly ApplicationDbContext _db;private readonly UserManager<ApplicationUser> _users;public NotificationsController(ApplicationDbContext db,UserManager<ApplicationUser> users){_db=db;_users=users;}
 public async Task<IActionResult> Index(){var u=await _users.GetUserAsync(User);if(u==null)return Challenge();await CreateExpiryNotificationIfNeeded(u.Id);if(User.IsInRole("Admin")) ViewBag.Users=await _users.Users.AsNoTracking().Where(x=>x.IsActive).OrderBy(x=>x.LastName).ThenBy(x=>x.FirstName).Take(500).ToListAsync();var items=await _db.UserNotifications.AsNoTracking().Where(x=>x.UserId==u.Id).OrderByDescending(x=>x.CreatedAt).Take(200).ToListAsync();return View(items);}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> MarkRead(int id){var u=await _users.GetUserAsync(User);if(u==null)return Challenge();var n=await _db.UserNotifications.FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==u.Id);if(n==null)return NotFound();n.IsRead=true;n.ReadAt=DateTime.UtcNow;await _db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> MarkAllRead(){var u=await _users.GetUserAsync(User);if(u==null)return Challenge();await _db.UserNotifications.Where(x=>x.UserId==u.Id&&!x.IsRead).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.IsRead,true).SetProperty(x=>x.ReadAt,(DateTime?)DateTime.UtcNow));return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken,Authorize(Roles="Admin")]
 public async Task<IActionResult> Create(string userId,string title,string message,string? url,string type="Info"){if(string.IsNullOrWhiteSpace(userId)||string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(message))return BadRequest("Title and message are required.");if(!await _users.Users.AnyAsync(x=>x.Id==userId))return NotFound();_db.UserNotifications.Add(new UserNotification{UserId=userId,Title=title.Trim(),Message=message.Trim(),Url=string.IsNullOrWhiteSpace(url)?null:url.Trim(),Type=string.IsNullOrWhiteSpace(type)?"Info":type.Trim()});await _db.SaveChangesAsync();return RedirectToAction(nameof(Index));}

 [HttpGet]public async Task<IActionResult> UnreadCount(){var u=await _users.GetUserAsync(User);if(u==null)return Unauthorized();return Json(new{count=await _db.UserNotifications.CountAsync(x=>x.UserId==u.Id&&!x.IsRead)});}
 private async Task CreateExpiryNotificationIfNeeded(string userId)
 {
     var member=await _db.Members.AsNoTracking().FirstOrDefaultAsync(x=>x.UserId==userId&&x.IsActive); if(member==null)return;
     var today=DateTime.UtcNow.Date; var expiry=await _db.Subscriptions.AsNoTracking().Where(x=>x.MemberId==member.Id&&x.IsActive&&x.EndDate>=today&&x.EndDate<=today.AddDays(7)).OrderBy(x=>x.EndDate).Select(x=>x.EndDate).FirstOrDefaultAsync();
     if(expiry==default)return;
     var marker=$"Membership expires on {expiry:yyyy-MM-dd}";
     if(!await _db.UserNotifications.AnyAsync(x=>x.UserId==userId&&x.Title=="Membership expiry reminder"&&x.Message==marker&&x.CreatedAt>=today))
     { _db.UserNotifications.Add(new UserNotification{UserId=userId,Title="Membership expiry reminder",Message=marker,Type="Warning",Url="/Member/Dashboard"}); await _db.SaveChangesAsync(); }
 }
}
