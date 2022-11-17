using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace SongBook.Web.Models;

[PublicAPI]
public sealed class Config
{
    [Required]
    [MinLength(1)]
    public string ApplicationName { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleSheetId { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRangeIndex { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRangeChords { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRangePostfix { get; init; } = null!;

    public Dictionary<string, string>? GoogleCredential { get; init; }

    public string? GoogleCredentialJson { get; init; }

    [Required]
    [MinLength(1)]
    public string SavePath { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string SystemTimeZoneIdLogs { get; init; } = null!;
}