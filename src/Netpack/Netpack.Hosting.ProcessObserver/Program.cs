// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using Newtonsoft.Json;

if (args.Length != 2)
{
    Console.WriteLine("Использование: <ID процесса> <Папка с processes.json>");
    return;
}

if (!int.TryParse(args[0], out var processIdToWatch))
{
    Console.WriteLine("Некорректный ID процесса.");
    return;
}

var folderPath = args[1];
var jsonFilePath = Path.Combine(folderPath, "processes.json");

try
{
    using var processToWatch = Process.GetProcessById(processIdToWatch);
    processToWatch.WaitForExit();
}
catch (ArgumentException)
{
    Console.WriteLine("Процесс с указанным ID не найден.");
    return;
}

var processIdsToKill = JsonConvert.DeserializeObject<List<ObserverProcess>>(File.ReadAllText(jsonFilePath));

if (processIdsToKill == null)
{
    return;
}

foreach (var pid in processIdsToKill)
{
    try
    {
        var processToKill = Process.GetProcessById(pid.id);
        processToKill.Kill();
        Console.WriteLine($"Процесс с ID {pid} был остановлен.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Не удалось остановить процесс с ID {pid}: {ex.Message}");
    }
}

try
{
    Directory.Delete(folderPath, true);
    Console.WriteLine($"Папка {folderPath} была удалена.");
}
catch (Exception ex)
{
    Console.WriteLine($"Не удалось удалить папку {folderPath}: {ex.Message}");
}

