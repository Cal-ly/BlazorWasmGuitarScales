namespace BlazorWasmGuitarScales.Services;

/// <summary>
/// Provides services for working with musical scales.
/// </summary>
public static class ScaleService
{
    /// <summary>
    /// Gets the chromatic scale notes.
    /// </summary>
    public static NoteName[] Chromatic => Enum.GetValues<NoteName>();

    private static readonly Dictionary<ScaleType, List<ScaleStep>> ScalePatterns = new()
    {
        { ScaleType.Major,     new() { ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half } },
        { ScaleType.Minor,     new() { ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole } },
        { ScaleType.Pentatonic,new() { ScaleStep.Whole, ScaleStep.Whole, ScaleStep.WholeHalf, ScaleStep.Whole, ScaleStep.WholeHalf } }
    };

    /// <summary>
    /// Gets the available keys.
    /// </summary>
    /// <returns>A list of available keys.</returns>
    public static List<NoteName> GetAvailableKeys() => Chromatic.ToList();

    /// <summary>
    /// Gets the available scales.
    /// </summary>
    /// <returns>A list of available scales.</returns>
    public static List<ScaleType> GetAvailableScales() => ScalePatterns.Keys.ToList();

    /// <summary>
    /// Gets the note from the root note with a specified semitone offset.
    /// </summary>
    /// <param name="rootNote">The root note.</param>
    /// <param name="semitoneOffset">The semitone offset.</param>
    /// <returns>The note at the specified semitone offset from the root note.</returns>
    public static NoteName GetNoteFrom(NoteName rootNote, int semitoneOffset)
    {
        int startIndex = (int)rootNote;
        return Chromatic[(startIndex + semitoneOffset) % 12];
    }

    /// <summary>
    /// Gets the notes in a specified scale.
    /// </summary>
    /// <param name="root">The root note of the scale.</param>
    /// <param name="scale">The type of scale.</param>
    /// <returns>A set of notes in the specified scale.</returns>
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

/// <summary>
/// Represents musical note names.
/// </summary>
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

/// <summary>
/// Represents types of musical scales.
/// </summary>
public enum ScaleType
{
    Major,
    Minor,
    Pentatonic
}

/// <summary>
/// Represents steps in a musical scale.
/// </summary>
public enum ScaleStep
{
    None,
    Half,
    Whole,
    WholeHalf,
    Double
}
