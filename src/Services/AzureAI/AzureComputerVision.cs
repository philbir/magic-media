using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Serilog;

namespace MagicMedia.AzureAI
{
    public class AzureComputerVision : ICloudAIMediaAnalyser
    {
        public AzureComputerVision(Func<ComputerVisionClient> computerVisionClientFunc)
        {
            _computerVisionClientFunc = computerVisionClientFunc;
        }

        private IList<VisualFeatureTypes?> Features =>
            new List<VisualFeatureTypes?>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Color, VisualFeatureTypes.Objects, VisualFeatureTypes.Adult,
            VisualFeatureTypes.Tags, VisualFeatureTypes.Brands
        };

        public AISource Source => AISource.AzureCV;

        private readonly Func<ComputerVisionClient> _computerVisionClientFunc;

        public async Task<MediaAI> AnalyseImageAsync(
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

                return ToMediaAI(analysis);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in analyse image");
                throw;
            }
        }

        private MediaAI ToMediaAI(ImageAnalysis analysis)
        {
            var mediaAI = new MediaAI();

            mediaAI.Tags = analysis.Tags.Select(x => new MediaAITag
            {
                Name = x.Name,
                Confidence = x.Confidence * 100,
                Source = Source
            });

            mediaAI.Objects = analysis.Objects.Select(x => new MediaAIObject
            {
                Name = x.ObjectProperty,
                Confidence = x.Confidence * 100,
                Source = Source,
                Box = MapImageBox(x.Rectangle, analysis.Metadata)
            });

            mediaAI.Colors = MapColors(analysis.Color);
            mediaAI.Caption = MapCaption(analysis.Description);
            mediaAI.SourceInfo = GetSourceInfo(analysis);

            return mediaAI;
        }

        private IEnumerable<MediaAISourceInfo> GetSourceInfo(ImageAnalysis analysis)
        {
            var sourceInfo = new MediaAISourceInfo
            {
                AnalysisDate = DateTime.UtcNow,
                Source = Source,
                Data = new() { ["RequestId"] = analysis.RequestId }
            };

            return new List<MediaAISourceInfo> { sourceInfo };
        }

        private MediaAICaption? MapCaption(ImageDescriptionDetails? description)
        {
            if (description is { } desc && desc.Captions.Any())
            {
                ImageCaption capt = desc.Captions.First();

                return new MediaAICaption
                {
                    Text = capt.Text,
                    Confidence = capt.Confidence * 100
                };
            }

            return null;
        }

        private ImageBox MapImageBox(BoundingRect rectangle, ImageMetadata metadata)
        {
            var imgBox = new ImageBox();
            imgBox.Top = rectangle.Y;
            imgBox.Left = rectangle.X;
            imgBox.Bottom = metadata.Height - rectangle.H - imgBox.Top;
            imgBox.Right = imgBox.Left + rectangle.W;

            return imgBox;
        }

        private MediaAIColors? MapColors(ColorInfo? colors)
        {
            if (colors != null)
            {
                return new MediaAIColors
                {
                    Accent = colors.AccentColor,
                    DominantBackground = colors.DominantColorBackground,
                    DominantForeground = colors.DominantColorForeground,
                    IsBackWhite = colors.IsBWImg
                };
            }

            return null;
        }
    }
}
