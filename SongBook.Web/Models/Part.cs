﻿using System.Collections.Generic;

namespace SongBook.Web.Models
{
    public sealed class Part
    {
        public string Name => HalfBars[0].Part;

        public readonly IList<HalfBarData> HalfBars;

        internal Part(HalfBarData halfBar) => HalfBars = new List<HalfBarData> { halfBar };
    }
}
