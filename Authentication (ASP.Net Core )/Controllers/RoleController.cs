using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Authentication__ASP.Net_Core__.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication__ASP.Net_Core__.Controllers
{

    [Route("api/v1/roles/")]
    [ApiController]
    //[Authorize(Roles ="Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleRepository.GetAllRolesAsync();
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("CreateRole")]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleDTO roleDto)
        {
            var role = await _roleRepository.CreateRoleAsync(roleDto);
            return Ok(role);
        }

        [HttpPut("UpdateRole/{roleId}")]
        public async Task<IActionResult> UpdateRole(int roleId , [FromBody] RoleDTO roleDto)
        {
            var role = await _roleRepository.UpdateRoleAsync(roleId, roleDto);
            return Ok(role);
        }


        [HttpPost("AssignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignUserRoleDTO assignUserRoleDTO)
        {
            var assignedRole = await _roleRepository.AssignRoleToUserAsync(assignUserRoleDTO);
            if (assignedRole)
            {
                return Ok("Role Assigned Successfully");
            }

            return BadRequest("Failed to Assign Role");

        }


        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var response = await _roleRepository.DeleteRoleAsync(roleId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
