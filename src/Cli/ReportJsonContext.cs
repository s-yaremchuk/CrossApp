using System.Text.Json.Serialization;
using Core;

[JsonSerializable(typeof(EnvironmentReport))]
internal partial class ReportJsonContext : JsonSerializerContext;
