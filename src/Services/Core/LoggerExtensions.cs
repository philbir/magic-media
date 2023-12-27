using System;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Discovery;

public static partial class LoggerExtensions
{
    #region FileSystemSourceDiscovery

    [LoggerMessage(LogLevel.Information, "Discover media in '{path}' with pattern '{pattern}'")]
    public static partial void DiscoverMediaWithPathAndPattern(this ILogger logger, string path, string pattern);

    [LoggerMessage(LogLevel.Information, "{count} media found in path '{path}' with pattern '{pattern}'")]
    public static partial void MediaFoundWithPathAndPattern(this ILogger logger, int count, string path, string pattern);

    #endregion

    #region DuplicateMediaGuard

    [LoggerMessage(LogLevel.Information, "Media identifier already exists: {Identifier}")]
    public static partial void MediaIdentifierAllreadyExsists(this ILogger logger, string Identifier);

    #endregion

    #region CheckDuplicateTask

    [LoggerMessage(LogLevel.Warning, "Duplicate media found, stop processing")]
    public static partial void DuplicateMediaFound(this ILogger logger, string id);

    [LoggerMessage(LogLevel.Information, "Move duplicate file from:{from} to:{to} ")]
    public static partial void MoveDuplicateFile(this ILogger logger, string from, string to);

    [LoggerMessage(LogLevel.Warning, "Could not move file to:{to}")]
    public static partial void CouldNotMoveFile(this ILogger logger, string to, Exception ex);

    #endregion

    #region MediaSourceScanner

    [LoggerMessage(LogLevel.Error, "Error while processing file: {id}")]
    public static partial void ErrorProcessingFile(this ILogger logger, string id, Exception ex);

    #endregion


    #region  MediaSourcePreConverter

    [LoggerMessage(LogLevel.Information, "{count} media found for preconversion")]
    public static partial void MediaFoundforPreConversion(this ILogger logger, int count);

    #endregion
}
