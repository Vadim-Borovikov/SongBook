namespace SongBook.Web.Models;

internal readonly struct Tune
{
    public static readonly byte Limit = (byte)Chord.Semitones.Length;

    public readonly byte Value;

    public Tune(byte v) => Value = v;

    public Tune Invert() => Invert(Value);

    public static Tune operator+(Tune tune, sbyte delta) => new((byte) ((Limit + tune.Value + delta) % Limit));
    public static Tune operator-(Tune tune, sbyte delta) => tune + (sbyte)-delta;
    public static sbyte operator-(Tune first, Tune second) => (sbyte) (first.Value - second.Value);

    private static Tune Invert(byte v) => new((byte)((Limit - v) % Limit));
}