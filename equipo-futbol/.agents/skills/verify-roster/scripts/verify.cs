#:project ../../../../api/ClubDeBarrio.Api.csproj
#:property PublishAot=false

using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

using var stop = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; stop.Cancel(); };
var temp = Directory.CreateTempSubdirectory("club-de-barrio-");
var database = Path.Combine(temp.FullName, "club.db");
var apiFolder = Path.GetFullPath("../../../../api", Path.GetDirectoryName(SourceFile())!);
try
{
    var ct = stop.Token;
    int playerId, coachId, availableShirt;
    await using (var api = await LocalApi.StartAsync(apiFolder, database, ct))
    {
        var players = await Request(api.Http, HttpMethod.Get, "api/players", HttpStatusCode.OK, ct);
        var taken = players.EnumerateArray().Select(p => p.GetProperty("shirtNumber").GetInt32()).ToHashSet();
        var free = Enumerable.Range(1, 99).Except(taken).Take(3).ToArray();
        Require(taken.Count > 0 && free.Length == 3, "La plantilla inicial necesita un dorsal ocupado y tres libres.");
        availableShirt = free[2];

        var newPlayer = new { name = "Olga Ensayo", birthDate = "2001-06-12", nationality = "España",
            position = "midfielder", shirtNumber = free[0] };
        var created = await Request(api.Http, HttpMethod.Post, "api/players", HttpStatusCode.Created, ct, newPlayer);
        playerId = created.GetProperty("id").GetInt32();
        var playerUrl = $"api/players/{playerId}";
        await Request(api.Http, HttpMethod.Put, playerUrl, HttpStatusCode.OK, ct,
            newPlayer with { shirtNumber = free[1], position = "defender" });
        var player = await Request(api.Http, HttpMethod.Get, playerUrl, HttpStatusCode.OK, ct);
        Require(player.GetProperty("shirtNumber").GetInt32() == free[1] &&
            player.GetProperty("position").GetString() == "defender", "La edición del jugador no se guardó.");
        Console.WriteLine("OK · Alta, consulta y edición de un jugador por REST.");

        await Request(api.Http, HttpMethod.Put, playerUrl, HttpStatusCode.Conflict, ct,
            newPlayer with { shirtNumber = taken.First(), position = "forward" });
        player = await Request(api.Http, HttpMethod.Get, playerUrl, HttpStatusCode.OK, ct);
        Require(player.GetProperty("shirtNumber").GetInt32() == free[1] &&
            player.GetProperty("position").GetString() == "defender", "El conflicto alteró la ficha original.");
        Console.WriteLine("OK · El dorsal ocupado devuelve 409 y conserva la ficha anterior.");

        var newCoach = new { name = "Iria Ensayo", birthDate = "1990-05-03", nationality = "España",
            role = "assistantCoach" };
        var coach = await Request(api.Http, HttpMethod.Post, "api/coaches", HttpStatusCode.Created, ct, newCoach);
        coachId = coach.GetProperty("id").GetInt32();
        var coachUrl = $"api/coaches/{coachId}";
        await Request(api.Http, HttpMethod.Put, coachUrl, HttpStatusCode.OK, ct,
            newCoach with { role = "headCoach" });
        coach = await Request(api.Http, HttpMethod.Get, coachUrl, HttpStatusCode.OK, ct);
        Require(coach.GetProperty("role").GetString() == "headCoach", "La función del técnico no se guardó.");
        Console.WriteLine("OK · Alta, consulta y edición de un entrenador por REST.");

        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(api.Http.BaseAddress!, "mcp"),
            TransportMode = HttpTransportMode.StreamableHttp
        });
        await using var mcp = await McpClient.CreateAsync(transport, cancellationToken: ct);
        var tools = await mcp.ListToolsAsync(cancellationToken: ct);
        string[] expected = ["list_players", "get_player", "create_player", "update_player", "delete_player",
            "list_coaches", "get_coach", "create_coach", "update_coach", "delete_coach"];
        var missing = expected.Except(tools.Select(tool => tool.Name)).ToArray();
        Require(missing.Length == 0, "Faltan herramientas MCP: " + string.Join(", ", missing));
        var result = await mcp.CallToolAsync("update_player",
            new Dictionary<string, object?> { ["id"] = playerId, ["shirtNumber"] = availableShirt },
            cancellationToken: ct);
        Require(result.IsError != true, "MCP rechazó el cambio: " +
            string.Join(" ", result.Content.OfType<TextContentBlock>().Select(block => block.Text)));
        player = await Request(api.Http, HttpMethod.Get, playerUrl, HttpStatusCode.OK, ct);
        Require(player.GetProperty("shirtNumber").GetInt32() == availableShirt &&
            player.GetProperty("position").GetString() == "defender", "REST no refleja el cambio parcial de MCP.");
        Console.WriteLine("OK · Descubrimiento de diez herramientas y cambio de dorsal por MCP visible desde REST.");
    }

    await using (var restarted = await LocalApi.StartAsync(apiFolder, database, ct))
    {
        var playerUrl = $"api/players/{playerId}";
        var coachUrl = $"api/coaches/{coachId}";
        var player = await Request(restarted.Http, HttpMethod.Get, playerUrl, HttpStatusCode.OK, ct);
        var coach = await Request(restarted.Http, HttpMethod.Get, coachUrl, HttpStatusCode.OK, ct);
        Require(player.GetProperty("shirtNumber").GetInt32() == availableShirt &&
            coach.GetProperty("role").GetString() == "headCoach", "Los cambios no persistieron tras el reinicio.");
        Console.WriteLine("OK · Los cambios del jugador y del entrenador persisten tras reiniciar.");

        foreach (var url in new[] { playerUrl, coachUrl })
        {
            await Request(restarted.Http, HttpMethod.Delete, url, HttpStatusCode.NoContent, ct);
            await Request(restarted.Http, HttpMethod.Get, url, HttpStatusCode.NotFound, ct);
        }
        Console.WriteLine("OK · Baja de ambas fichas y consulta posterior con 404.");
    }
    Console.WriteLine("Pendiente de comprobación manual: chat con Ollama y actualización visual en React.");
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine($"FALLO · {error.Message}");
    Console.Error.WriteLine("Solo se completaron los pasos marcados como OK.");
    return 1;
}
finally
{
    // Only remove the database and SQLite sidecar files created for this run.
    foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" }) File.Delete(database + suffix);
    temp.Delete();
}

static string SourceFile([CallerFilePath] string path = "") => path;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static async Task<JsonElement> Request(HttpClient http, HttpMethod method, string path, HttpStatusCode expected,
    CancellationToken ct, object? body = null)
{
    using var request = new HttpRequestMessage(method, path);
    if (body is not null) request.Content = JsonContent.Create(body);
    using var response = await http.SendAsync(request, ct);
    var text = await response.Content.ReadAsStringAsync(ct);
    if (response.StatusCode != expected)
        throw new InvalidOperationException($"{method} /{path}: se esperaba {(int)expected}, se recibió {(int)response.StatusCode}. {text}");
    if (string.IsNullOrWhiteSpace(text)) return default;
    return JsonSerializer.Deserialize<JsonElement>(text);
}

sealed class LocalApi(Process process, Uri address) : IAsyncDisposable
{
    public HttpClient Http { get; } = new() { BaseAddress = address, Timeout = TimeSpan.FromSeconds(20) };

    public static async Task<LocalApi> StartAsync(string apiFolder, string database, CancellationToken ct)
    {
        var dll = Path.Combine(apiFolder, "bin", "Debug", "net10.0", "ClubDeBarrio.Api.dll");
        var info = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = apiFolder, UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        foreach (var arg in new[] { "exec", dll, "--urls", "http://127.0.0.1:0", "--Roster:DatabasePath", database,
            "--Logging:LogLevel:Microsoft.Hosting.Lifetime", "Information",
            "--Logging:Console:FormatterName", "simple", "--Logging:Console:FormatterOptions:ColorBehavior", "Disabled" })
            info.ArgumentList.Add(arg);

        // Observe the address allocated to this child; never probe a pre-existing API on a fixed port.
        var ready = new TaskCompletionSource<Uri>(TaskCreationOptions.RunContinuationsAsynchronously);
        var child = new Process { StartInfo = info, EnableRaisingEvents = true };
        child.OutputDataReceived += (_, eventArgs) =>
        {
            const string marker = "Now listening on: ";
            var line = eventArgs.Data;
            if (line is null) return;
            int start = line.IndexOf(marker, StringComparison.Ordinal);
            if (start >= 0 && Uri.TryCreate(line[(start + marker.Length)..].Trim(), UriKind.Absolute, out var uri)
                && uri.Scheme == "http" && uri.Host == "127.0.0.1" && uri.Port > 0)
                ready.TrySetResult(uri);
        };
        child.ErrorDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data is not null) Console.Error.WriteLine(eventArgs.Data);
        };
        child.Exited += (_, _) => ready.TrySetException(new InvalidOperationException(
            $"La instancia de verificación terminó con código {child.ExitCode}."));

        child.Start();
        try
        {
            child.BeginOutputReadLine();
            child.BeginErrorReadLine();
            var address = await ready.Task.WaitAsync(TimeSpan.FromSeconds(45), ct);
            if (!File.Exists(database))
                throw new InvalidOperationException("La API no creó la base temporal. Se interrumpe la comprobación.");
            return new LocalApi(child, address);
        }
        catch
        {
            await StopAsync(child);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        Http.Dispose();
        await StopAsync(process);
    }

    private static async Task StopAsync(Process child)
    {
        using (child)
        {
            if (!child.HasExited) child.Kill(entireProcessTree: true);
            await child.WaitForExitAsync();
        }
    }
}
