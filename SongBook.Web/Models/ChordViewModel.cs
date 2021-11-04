using System;

namespace SongBook.Web.Models
{
    public sealed class ChordViewModel : IEquatable<ChordViewModel>
    {
        public Fingering Fingering => _chord?.Fingerings[_option];
        public bool IsSimple => (_chord == null) || (_chord.IsSimple && (_option == 0));

        public ChordViewModel(Chord chord, int option)
        {
            _chord = chord;
            _option = option;
        }

        public override string ToString()
        {
            if (_chord == null)
            {
                return null;
            }

            string chord = _chord.ToString();
            if (_option > 0)
            {
                chord += $"({_option + 1})";
            }
            return chord;
        }

        public bool Equals(ChordViewModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(_chord, other._chord) && (_option == other._option);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is ChordViewModel ñhordViewModel && Equals(ñhordViewModel);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_chord == null ? 0 : _chord.GetHashCode()) * 397) ^ _option;
            }
        }

        private readonly Chord _chord;
        private readonly int _option;
    }
}