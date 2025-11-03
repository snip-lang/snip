using Snip.AST;
using System.Text;

namespace Snip.Evaluator;

public static class FileModule
{
    public static Module CreateModule()
    {
        var module = new Module("file");
        
        // File operations
        module.Export("read_text", Value.NativeFunction(new NativeFunctionValue("read_text", ReadText)));
        module.Export("write_text", Value.NativeFunction(new NativeFunctionValue("write_text", WriteText)));
        module.Export("read_bytes", Value.NativeFunction(new NativeFunctionValue("read_bytes", ReadBytes)));
        module.Export("write_bytes", Value.NativeFunction(new NativeFunctionValue("write_bytes", WriteBytes)));
        module.Export("append_text", Value.NativeFunction(new NativeFunctionValue("append_text", AppendText)));
        module.Export("append_bytes", Value.NativeFunction(new NativeFunctionValue("append_bytes", AppendBytes)));
        
        // File info
        module.Export("exists", Value.NativeFunction(new NativeFunctionValue("exists", Exists)));
        module.Export("size", Value.NativeFunction(new NativeFunctionValue("size", Size)));
        module.Export("is_file", Value.NativeFunction(new NativeFunctionValue("is_file", IsFile)));
        module.Export("is_directory", Value.NativeFunction(new NativeFunctionValue("is_directory", IsDirectory)));
        module.Export("get_modified_time", Value.NativeFunction(new NativeFunctionValue("get_modified_time", GetModifiedTime)));
        module.Export("get_created_time", Value.NativeFunction(new NativeFunctionValue("get_created_time", GetCreatedTime)));
        
        // Directory operations
        module.Export("list_directory", Value.NativeFunction(new NativeFunctionValue("list_directory", ListDirectory)));
        module.Export("create_directory", Value.NativeFunction(new NativeFunctionValue("create_directory", CreateDirectory)));
        module.Export("delete_directory", Value.NativeFunction(new NativeFunctionValue("delete_directory", DeleteDirectory)));
        module.Export("delete_file", Value.NativeFunction(new NativeFunctionValue("delete_file", DeleteFile)));
        
        // File manipulation
        module.Export("copy", Value.NativeFunction(new NativeFunctionValue("copy", Copy)));
        module.Export("move", Value.NativeFunction(new NativeFunctionValue("move", Move)));
        module.Export("rename", Value.NativeFunction(new NativeFunctionValue("rename", Rename)));
        
        // Path operations
        module.Export("join", Value.NativeFunction(new NativeFunctionValue("join", Join)));
        module.Export("split", Value.NativeFunction(new NativeFunctionValue("split", Split)));
        module.Export("get_extension", Value.NativeFunction(new NativeFunctionValue("get_extension", GetExtension)));
        module.Export("get_filename", Value.NativeFunction(new NativeFunctionValue("get_filename", GetFilename)));
        module.Export("get_directory", Value.NativeFunction(new NativeFunctionValue("get_directory", GetDirectory)));
        module.Export("get_absolute_path", Value.NativeFunction(new NativeFunctionValue("get_absolute_path", GetAbsolutePath)));
        
        return module;
    }
    
    private static Value ReadText(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "read_text");
        var path = ValidationHelpers.GetString(args, 0);
        var encoding = args.Count > 1 ? ValidationHelpers.GetString(args, 1) : "utf-8";
        
        try
        {
            var textEncoding = GetEncoding(encoding);
            var content = File.ReadAllText(path, textEncoding);
            return Value.String(content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to read file '{path}': {ex.Message}");
        }
    }
    
    private static Value WriteText(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "write_text");
        var path = ValidationHelpers.GetString(args, 0);
        var content = ValidationHelpers.GetString(args, 1);
        var encoding = args.Count > 2 ? ValidationHelpers.GetString(args, 2) : "utf-8";
        
        try
        {
            var textEncoding = GetEncoding(encoding);
            File.WriteAllText(path, content, textEncoding);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to write file '{path}': {ex.Message}");
        }
    }
    
    private static Value ReadBytes(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "read_bytes");
        var path = ValidationHelpers.GetString(args, 0);
        
        try
        {
            var bytes = File.ReadAllBytes(path);
            var array = new List<Value>();
            foreach (var b in bytes)
            {
                array.Add(Value.Number(b));
            }
            return Value.Array(array);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to read bytes from '{path}': {ex.Message}");
        }
    }
    
    private static Value WriteBytes(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "write_bytes");
        var path = ValidationHelpers.GetString(args, 0);
        var data = ValidationHelpers.GetArray(args, 1);
        
        try
        {
            var bytes = new byte[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                var num = ValidationHelpers.GetNumber(new List<Value> { data[i] }, 0);
                if (num < 0 || num > 255)
                    throw new Exception($"Byte value must be between 0 and 255, got {num}");
                bytes[i] = (byte)num;
            }
            File.WriteAllBytes(path, bytes);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to write bytes to '{path}': {ex.Message}");
        }
    }
    
    private static Value AppendText(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "append_text");
        var path = ValidationHelpers.GetString(args, 0);
        var content = ValidationHelpers.GetString(args, 1);
        var encoding = args.Count > 2 ? ValidationHelpers.GetString(args, 2) : "utf-8";
        
        try
        {
            var textEncoding = GetEncoding(encoding);
            File.AppendAllText(path, content, textEncoding);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to append to file '{path}': {ex.Message}");
        }
    }
    
    private static Value AppendBytes(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "append_bytes");
        var path = ValidationHelpers.GetString(args, 0);
        var data = ValidationHelpers.GetArray(args, 1);
        
        try
        {
            var bytes = new byte[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                var num = ValidationHelpers.GetNumber(new List<Value> { data[i] }, 0);
                if (num < 0 || num > 255)
                    throw new Exception($"Byte value must be between 0 and 255, got {num}");
                bytes[i] = (byte)num;
            }
            File.AppendAllText(path, Encoding.UTF8.GetString(bytes));
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to append bytes to '{path}': {ex.Message}");
        }
    }
    
    private static Value Exists(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "exists");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(File.Exists(path) || Directory.Exists(path));
    }
    
    private static Value Size(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "size");
        var path = ValidationHelpers.GetString(args, 0);
        
        try
        {
            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                return Value.Number(fileInfo.Length);
            }
            else
            {
                throw new Exception($"File not found: {path}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get size of '{path}': {ex.Message}");
        }
    }
    
    private static Value IsFile(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "is_file");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(File.Exists(path));
    }
    
    private static Value IsDirectory(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "is_directory");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(Directory.Exists(path));
    }
    
    private static Value GetModifiedTime(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "get_modified_time");
        var path = ValidationHelpers.GetString(args, 0);
        
        try
        {
            var fileInfo = new FileInfo(path);
            return Value.Number(((DateTimeOffset)fileInfo.LastWriteTime).ToUnixTimeSeconds());
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get modified time of '{path}': {ex.Message}");
        }
    }
    
    private static Value GetCreatedTime(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "get_created_time");
        var path = ValidationHelpers.GetString(args, 0);
        
        try
        {
            var fileInfo = new FileInfo(path);
            return Value.Number(((DateTimeOffset)fileInfo.CreationTime).ToUnixTimeSeconds());
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get created time of '{path}': {ex.Message}");
        }
    }
    
    private static Value ListDirectory(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "list_directory");
        var path = ValidationHelpers.GetString(args, 0);
        var recursive = args.Count > 1 ? ValidationHelpers.GetBoolean(args, 1) : false;
        
        try
        {
            var files = new List<Value>();
            
            if (recursive)
            {
                foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    files.Add(Value.String(file));
                }
            }
            else
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    files.Add(Value.String(file));
                }
            }
            
            return Value.Array(files);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to list directory '{path}': {ex.Message}");
        }
    }
    
    private static Value CreateDirectory(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "create_directory");
        var path = ValidationHelpers.GetString(args, 0);
        
        try
        {
            Directory.CreateDirectory(path);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create directory '{path}': {ex.Message}");
        }
    }
    
    private static Value DeleteDirectory(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "delete_directory");
        var path = ValidationHelpers.GetString(args, 0);
        var recursive = args.Count > 1 ? ValidationHelpers.GetBoolean(args, 1) : false;
        
        try
        {
            if (recursive)
            {
                Directory.Delete(path, true);
            }
            else
            {
                Directory.Delete(path);
            }
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete directory '{path}': {ex.Message}");
        }
    }
    
    private static Value DeleteFile(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "delete_file");
        var path = ValidationHelpers.GetString(args, 0);
        
        try
        {
            File.Delete(path);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete file '{path}': {ex.Message}");
        }
    }
    
    private static Value Copy(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "copy");
        var source = ValidationHelpers.GetString(args, 0);
        var destination = ValidationHelpers.GetString(args, 1);
        var overwrite = args.Count > 2 ? ValidationHelpers.GetBoolean(args, 2) : false;
        
        try
        {
            File.Copy(source, destination, overwrite);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to copy '{source}' to '{destination}': {ex.Message}");
        }
    }
    
    private static Value Move(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "move");
        var source = ValidationHelpers.GetString(args, 0);
        var destination = ValidationHelpers.GetString(args, 1);
        
        try
        {
            File.Move(source, destination);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to move '{source}' to '{destination}': {ex.Message}");
        }
    }
    
    private static Value Rename(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "rename");
        var oldPath = ValidationHelpers.GetString(args, 0);
        var newPath = ValidationHelpers.GetString(args, 1);
        
        try
        {
            File.Move(oldPath, newPath);
            return Value.Boolean(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to rename '{oldPath}' to '{newPath}': {ex.Message}");
        }
    }
    
    private static Value Join(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "join");
        var parts = new List<string>();
        foreach (var arg in args)
        {
            parts.Add(ValidationHelpers.GetString(new List<Value> { arg }, 0));
        }
        return Value.String(Path.Combine(parts.ToArray()));
    }
    
    private static Value Split(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "split");
        var path = ValidationHelpers.GetString(args, 0);
        
        var directory = Path.GetDirectoryName(path) ?? "";
        var filename = Path.GetFileName(path);
        
        return Value.Array(new List<Value>
        {
            Value.String(directory),
            Value.String(filename)
        });
    }
    
    private static Value GetExtension(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "get_extension");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.String(Path.GetExtension(path));
    }
    
    private static Value GetFilename(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "get_filename");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.String(Path.GetFileNameWithoutExtension(path));
    }
    
    private static Value GetDirectory(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "get_directory");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.String(Path.GetDirectoryName(path) ?? "");
    }
    
    private static Value GetAbsolutePath(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "get_absolute_path");
        var path = ValidationHelpers.GetString(args, 0);
        return Value.String(Path.GetFullPath(path));
    }
    
    private static Encoding GetEncoding(string encodingName)
    {
        return encodingName.ToLower() switch
        {
            "utf-8" => Encoding.UTF8,
            "utf-16" => Encoding.Unicode,
            "utf-32" => Encoding.UTF32,
            "ascii" => Encoding.ASCII,
            "latin1" => Encoding.Latin1,
            _ => Encoding.UTF8
        };
    }
}