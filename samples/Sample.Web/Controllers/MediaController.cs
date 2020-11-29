using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.AzureAI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using SixLabors.ImageSharp;

namespace Sample.Web.Controllers
{
    [Authorize]
    [Route("media")]
    public class MediaController : Controller
    {
        private readonly SampleService _sampleService;
        private readonly AzureComputerVision _azureComputerVision;

        public MediaController(
            SampleService sampleService,
            AzureComputerVision azureComputerVision)
        {
            _sampleService = sampleService;
            _azureComputerVision = azureComputerVision;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<SampleMedia> samples = await _sampleService
                .GetSampleMediaListAsync();

            return View(samples);
        }

        [HttpGet]
        [Route("extractfaces/{id}")]
        public async Task<IActionResult> ExtractFaces(string id)
        {
            byte[] stream = _sampleService.GetMedia(id);
            var image = Image.Load(stream);

            var vm = new ExtractImageViewModel
            {
                Id = id,
                ImageDimension = new MediaDimension
                {
                    Height = image.Height,
                    Width = image.Width
                }
            };

            vm.Faces = await _sampleService
                .ExtractFacesAsync(id);

            return View(vm);
        }

        [HttpGet]
        [Route("azureai/{id}")]
        public async Task<IActionResult> AzureAI(string id)
        {
            byte[] data = _sampleService.GetMedia(id);
            var ms = new MemoryStream(data);

            ImageAnalysis analysis = await _azureComputerVision.AnalyseImageAsync(ms, default);

            return Ok(analysis);
        }


        [HttpGet]
        [Route("image/{id}")]
        public IActionResult GetImage(string id)
        {
            var data = _sampleService.GetMedia(id);
            return File(data, "application/octet-stream");
        }
    }

    public class ExtractImageViewModel
    {
        public IEnumerable<FaceDetectionInfo> Faces { get; set; }

        public MediaDimension ImageDimension { get; set; }

        public string Id { get; set; }
    }
}
