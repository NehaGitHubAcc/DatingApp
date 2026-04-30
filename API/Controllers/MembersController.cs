using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [Authorize] // this will protect all the endpoints in this controller, only authenticated users can access these endpoints
    public class MembersController(AppDbContext context): BaseApiController
    {
        [HttpGet] // localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();
            return members; // 200 + members
        }

        //[AllowAnonymous] // this will allow anonymous users to access this endpoint, even though the controller has [Authorize] attribute
        [Authorize] // this will allow only authenticated users to access this endpoint, even though the controller has [AllowAnonymous] attribute - not set on the controller, but set on the endpoint
        [HttpGet("{id}")] // localhost:5001/api/members/bob-id
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id);
            if (member == null) return NotFound(); // 404
            return member; // 200 + member
        }
    }
}
