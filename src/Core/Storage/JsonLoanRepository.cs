using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonLoanRepository : ILoanRepository
{
    private readonly string _path;

    public JsonLoanRepository(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = Path.GetFullPath(path);
    }

    public Loan? GetById(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return Load().Loans.FirstOrDefault(loan => loan.Id == id.Trim());
    }

    public void Update(Action<LoanRegister> change)
    {
        ArgumentNullException.ThrowIfNull(change);
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using var writerLock = new FileStream(_path + ".lock", FileMode.OpenOrCreate,
            FileAccess.ReadWrite, FileShare.None);
        var register = Load();
        change(register);
        var snapshots = register.Loans.Select(loan => loan.ToDto()).ToArray();
        var temporary = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(snapshots, LoanJsonContext.Default.LoanDtoArray));
            File.Move(temporary, _path, overwrite: true);
        }
        finally { File.Delete(temporary); }
    }

    private LoanRegister Load()
    {
        var register = new LoanRegister();
        if (!File.Exists(_path))
            return register;
        var snapshots = JsonSerializer.Deserialize(File.ReadAllText(_path), LoanJsonContext.Default.LoanDtoArray)
            ?? throw new InvalidDataException("Очікується масив записів про видачу.");
        foreach (var snapshot in snapshots)
            register.Restore(snapshot);
        return register;
    }
}

[JsonSerializable(typeof(LoanDto[]))]
internal partial class LoanJsonContext : JsonSerializerContext { }
