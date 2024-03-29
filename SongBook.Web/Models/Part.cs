﻿using System.Collections.Generic;
using GryphonUtilities.Extensions;

namespace SongBook.Web.Models;

internal sealed class Part
{
    public string Name => HalfBars[0].Part.GetValue(nameof(HalfBarData.Part));

    public readonly IList<HalfBarData> HalfBars;

    public Part(HalfBarData halfBar) => HalfBars = halfBar.WrapWithList();
}