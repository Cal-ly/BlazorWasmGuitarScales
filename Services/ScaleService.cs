namespace BlazorWasmGuitarScales.Services;

public static class ScaleService
{
    public static NoteName[] Chromatic => Enum.GetValues<NoteName>();

    private static readonly Dictionary<ScaleType, List<ScaleStep>> ScalePatterns = new()
    {
        { ScaleType.Major,     new() { ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half } },
        { ScaleType.Minor,     new() { ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole } },
        { ScaleType.Pentatonic,new() { ScaleStep.Whole, ScaleStep.Whole, ScaleStep.WholeHalf, ScaleStep.Whole, ScaleStep.WholeHalf } }
    };

    public static List<NoteName> GetAvailableKeys() => Chromatic.ToList();
    public static List<ScaleType> GetAvailableScales() => ScalePatterns.Keys.ToList();

    public static NoteName GetNoteFrom(NoteName rootNote, int semitoneOffset)
    {
        int startIndex = (int)rootNote;
        return Chromatic[(startIndex + semitoneOffset) % 12];
    }

    public static HashSet<NoteName> GetScaleNotes(NoteName root, ScaleType scale)
    {
        if (!ScalePatterns.TryGetValue(scale, out var steps)) return [];

        var result = new HashSet<NoteName> { root }; // Add the root note
        int semitoneOffset = 0;

        foreach (var step in steps)
        {
            semitoneOffset += step switch
            {
                ScaleStep.Half => 1,
                ScaleStep.Whole => 2,
                ScaleStep.WholeHalf => 3,
                _ => 0
            };

            result.Add(GetNoteFrom(root, semitoneOffset));
        }

        return result;
    }
}

public enum NoteName
{
    C = 0,
    Cs = 1,
    D = 2,
    Ds = 3,
    E = 4,
    F = 5,
    Fs = 6,
    G = 7,
    Gs = 8,
    A = 9,
    As = 10,
    B = 11
}

public enum ScaleType
{
    Major,
    Minor,
    Pentatonic
}

public enum ScaleStep
{
    None,
    Half,
    Whole,
    WholeHalf,
    Double
}
