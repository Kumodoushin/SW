using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Model;
using SWApi.Dao;
using SWApi.Requests;
using System;
using System.Threading.Tasks;

namespace SWApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public CharacterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CharacterCreationCommand characterForm)
        {
            var result = await _mediator.Send(characterForm);
            if (result.IsSuccessful)
            {
                return Accepted(result);
            }
            return BadRequest(result);
        }
        
        [HttpGet("{characterId:guid?}")]
        public async Task<IActionResult> Get([FromRoute] Guid characterId)
        {            
            if (characterId == Guid.Empty)
            {
                var charactersResult = await _mediator.Send(new CharactersQuery());
                return Ok(charactersResult);
            }
            var requestedCharacter = await _mediator.Send(new CharacterQuery(characterId));
            if(requestedCharacter.IsSuccessful)
            {
                return Ok(requestedCharacter);
            }            
            return NotFound(requestedCharacter);
        }
        
        [HttpPatch("{characterId:guid}")]
        public async Task<IActionResult> Patch([FromRoute] Guid characterId, [FromBody] CharacterUpdateForm characterForm)
        {
            var updateResult = await _mediator.Send(new CharacterUpdateCommand(characterId, characterForm));
            if (updateResult.IsSuccessful)
            {
                return Ok(updateResult);
            }
            if (updateResult.Errors.ContainsKey("Not Found"))
            {
                return NotFound(updateResult);
            }
            return BadRequest(updateResult);
        }
        
        [HttpDelete("{characterId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid characterId)
        {
            var deleteResult = await _mediator.Send(new CharacterDeletionCommand(characterId));
            
            if (deleteResult.IsSuccessful)
            {
                return Ok(deleteResult);
            }
            return NotFound(deleteResult);
        }
    }

   
    



   
}
