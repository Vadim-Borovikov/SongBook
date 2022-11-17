namespace SongBook.Web.Models;

public readonly struct Tune
{
    internal static readonly byte Limit = (byte)Chord.Semitones.Length;

    internal readonly byte Value;

    internal Tune(byte v) => Value = v;

    internal Tune Invert() => Invert(Value);

    public static Tune operator+(Tune tune, sbyte delta) => new((byte) ((Limit + tune.Value + delta) % Limit));
    public static Tune operator -(Tune tune, sbyte delta) => tune + (sbyte)-delta;
    public static sbyte operator-(Tune first, Tune second) => (sbyte) (first.Value - second.Value);

    private static Tune Invert(byte v) => new((byte)((Limit - v) % Limit));
}