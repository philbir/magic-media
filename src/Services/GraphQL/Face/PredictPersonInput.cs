namespace MagicMedia.GraphQL.Face;

public class PredictPersonInput
{
    public Guid FaceId { get; set; }

    public double? Distance { get; set; }
}

public class PredictPersonsByMediaInput
{
    public Guid MediaId { get; set; }

    public double? Distance { get; set; }
}
