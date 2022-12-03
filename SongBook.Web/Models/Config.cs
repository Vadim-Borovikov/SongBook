using GoogleSheetsManager;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace SongBook.Web.Models;

public sealed class Config : IConfigGoogleSheets
{
    public Dictionary<string, string>? Credential { get; init; }

    public string? CredentialJson { get; init; }

    [Required]
    [MinLength(1)]
    public string ApplicationName { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string TimeZoneId { get; init; } = null!;

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

    [Required]
    [MinLength(1)]
    public string SavePath { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string TimeZoneIdLogs { get; init; } = null!;
}