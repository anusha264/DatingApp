using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


// /api/users
[Authorize]
public class UsersController(IUserRespository userRespository, IMapper mapper) : BaseApiController
{
    // private readonly DataContext _Context;

    // public UsersController(DataContext context)
    // {
    //     _Context  = context;
    // }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRespository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}")]  // api/users/1
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRespository.GetMemberAsync(username);

        if (user == null) return NotFound();
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(username == null) return BadRequest("No username found in the token");

        var user = await userRespository.GetUserByUsernameAsync(username);
        mapper.Map(memberUpdateDto, user);
        if(await userRespository.SaveAllAsync()) return NoContent();
        return BadRequest("Update Failed");
    }
}