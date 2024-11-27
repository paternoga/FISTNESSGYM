using Microsoft.Extensions.Logging;
using FISTNESSGYM.Data;
using FISTNESSGYM.Models.database;
using System;
using Microsoft.EntityFrameworkCore;

public class DbLogger
{
    private readonly databaseContext _context;

    public DbLogger(databaseContext context)
    {
        _context = context;
    }

    public async Task LogToDatabaseAsync(LogLevel logLevel, string logMessage)
    {
        var dbLog = new Log
        {
            LogLevel = logLevel.ToString(),
            Message = logMessage,
            CreatedAt = DateTime.UtcNow
        };

        _context.Logs.Add(dbLog);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Log>> GetLogsAsync()
    {
        return await _context.Logs
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

}


