﻿namespace StatusExposed.Services;

public interface IEmailService
{
    void Send(string to, string subject, string html, string? from = null);

    void Send(IEnumerable<string> to, string subject, string html, string? from = null);
}