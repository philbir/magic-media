using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace MagicMedia.AzureAI
{
    public class AzureComputerVision
    {
        public AzureComputerVision(Func<ComputerVisionClient> computerVisionClientFunc)
        {
            _computerVisionClientFunc = computerVisionClientFunc;
        }

        private List<VisualFeatureTypes> Features =>
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Color, VisualFeatureTypes.Objects, VisualFeatureTypes.Adult,
            VisualFeatureTypes.Tags
        };

        private readonly Func<ComputerVisionClient> _computerVisionClientFunc;

        public async Task<ImageAnalysis> AnalyseImageAsync(
            Stream imageStream,
            CancellationToken cancellationToken)
        {
            ComputerVisionClient computerVision = _computerVisionClientFunc();
            try
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(
                    imageStream,
                    Features,
                    cancellationToken: cancellationToken);

                return analysis;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
