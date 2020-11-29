using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Route("api/person")]
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }


        [HttpGet]
        [Route("thumbnail/{id}")]
        public async Task<IActionResult> FaceThumbnailAsync(Guid id, CancellationToken cancellationToken)
        {
            MediaThumbnail? thumb = await _personService.TryGetFaceThumbnailAsync(id, cancellationToken);

            if ( thumb != null)
            {
                return new FileContentResult(thumb.Data, "image/jpg");
            }

            return NotFound();
        }
    }
}
