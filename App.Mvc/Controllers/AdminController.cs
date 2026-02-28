using App.Mvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Mvc.Controllers
{
    // Erişim yetkisini Moderator
    [Authorize(Roles = "SuperAdmin,Admin,Moderator")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AdminController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // --- SADECE ADMIN VE SUPERADMIN GİREBİLİR ---

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index()
        {
            var pendingUsers = await _dbContext.Users
                .Include(x => x.Role)
                .Where(x => !x.IsApproved && !x.IsRejected)
                .ToListAsync();
            return View(pendingUsers);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UserManagement()
        {
            var activeUsers = await _dbContext.Users.Include(x => x.Role)
                .Where(x => x.IsApproved && !x.IsRejected)
                .ToListAsync();
            return View(activeUsers);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.IsApproved = true;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var exciting = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

            // Güvenlik: SuperAdmin veya Admin reddedilemez
            if (exciting != null && exciting.Role.Name != "SuperAdmin" && exciting.Role.Name != "Admin")
            {
                exciting.IsRejected = true;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RejectedUsers()
        {
            var rejectedList = await _dbContext.Users
                .Include(x => x.Role)
                .Where(x => x.IsRejected)
                .ToListAsync();
            return View(rejectedList);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ChangeRole(int userId, string roleName)
        {
            var targetUser = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (targetUser == null) return NotFound();

            // KORUMA 1: SuperAdmin'e kimse dokunamaz
            if (targetUser.Role.Name == "SuperAdmin") return BadRequest("Sistem sahibine dokunulamaz.");

            // KORUMA 2: Sadece SuperAdmin birini Admin yapabilir
            if (roleName == "Admin" && currentUserRole != "SuperAdmin") return Forbid();

            var newRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (newRole != null)
            {
                targetUser.RoleId = newRole.Id;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("UserManagement");
        }

        // --- MODERATOR VE ÜSTÜ GİREBİLİR ---

        public IActionResult Moderator() => View();
               
    }
}