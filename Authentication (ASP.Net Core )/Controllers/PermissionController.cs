using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Authentication__ASP.Net_Core__.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Authentication__ASP.Net_Core__.Controllers
{

    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var response = await _permissionRepository.GetAllPermissionsAsync();
            return StatusCode(response.StatusCode, response);
        }

       


        [HttpPost("AddPermission")]
        public async Task<ActionResult> AddPermission([FromBody] ReqPermissionDTO reqPermissionDTO)
        {
            var permission = await _permissionRepository.AddPermissionAsync(reqPermissionDTO);
            return Ok(permission);
        }

        [HttpPost("AssignPermissionToUser")]
        public async Task<IActionResult> AssignPermissionToUser([FromBody] AssignPermissionToUserDTO assignPermissionToUserDTO)
        {
            var AssignedPerm = await _permissionRepository.AssignPermissionToUserAsync(assignPermissionToUserDTO);

            if (AssignedPerm)
            {
                return Ok("Permission Assigned");
            }

            return BadRequest("Failed to assign permission");
        }

        [HttpPut("UpdatePermission/{id}")]
        public async Task<IActionResult> UpdatePermission(int permissionId, [FromBody] UpdatePermDTO updatePermDTO)
        {
            var response = await _permissionRepository.UpdatePermissionAsync(permissionId, updatePermDTO);
            return StatusCode(response.StatusCode, response);
        }


        [HttpDelete("DeletePermission/{id}")]
        public async Task<IActionResult> DeletePermission(int permissionId)
        {
            var response = await _permissionRepository.DeletePermissionAsync(permissionId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
