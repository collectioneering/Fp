using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fp.Fs.CommandLine;

internal class FpFsRootCommand : RootCommand
{
    /// <summary>
    /// Default output folder name.
    /// </summary>
    public const string DefaultOutputFolderName = "fp_output";

    private readonly FileSystemSource? _fileSystemSource;
    private readonly FsProcessorFactory[] _factories;
    private readonly Option<bool> _registerProcessorsOption;
    private readonly Option<bool> _noExecuteCliOption;
    private readonly Option<bool> _debugOption;
    private readonly Option<int> _multithreadOption;
    private readonly Option<bool> _noopOption;
    private readonly Option<DirectoryInfo> _outdirOption;
    private readonly Option<bool> _preloadOption;
    private readonly Option<string> _processorOption;
    private readonly Argument<string[]> _inputsArgument;

    public FpFsRootCommand(FileSystemSource? fileSystemSource, IReadOnlyList<FsProcessorFactory> factories)
    {
        _fileSystemSource = fileSystemSource;
        _factories = factories.ToArray();
        _noExecuteCliOption = new Option<bool>(CommandLineFsProcessor.NO_EXECUTE_CLI) { Description = "If enabled, exit invocation without processing" };
        Add(_noExecuteCliOption);
        _registerProcessorsOption = new Option<bool>(CommandLineFsProcessor.REGISTER_PROCESSORS) { Description = "Register processors for scripting consumption" };
        Add(_registerProcessorsOption);
        _debugOption = new Option<bool>("-d", "--debug") { Description = "Enable debug" };
        Add(_debugOption);
        _multithreadOption = new Option<int>("-m", "--multithread") { HelpName = "worker-count", Description = "Use specified # of workers", DefaultValueFactory = static _ => 0 };
        Add(_multithreadOption);
        _noopOption = new Option<bool>("-n", "--nop") { Description = "No outputs" };
        Add(_noopOption);
        _outdirOption = new Option<DirectoryInfo>("-o", "--outdir") { HelpName = "output-directory", Description = "Output directory" };
        _outdirOption.AcceptLegalFilePathsOnly();
        Add(_outdirOption);
        _preloadOption = new Option<bool>("-p", "--preload") { Description = "Load all streams to memory" };
        Add(_preloadOption);
        _inputsArgument = new Argument<string[]>("input") { Description = "Files to process, or arguments to processor if after double-dash (--)", Arity = ArgumentArity.ZeroOrMore };
        Add(_inputsArgument);
        _processorOption = new Option<string>("--processor") { HelpName = "processor-name", Description = "Select specific processor to use by name" };
        Add(_processorOption);
        SetAction(ExecuteAsync);
        var descriptionSb = new StringBuilder();
        descriptionSb.AppendLine("Execute processors (automatically or specific by name) from the following:");
        AppendProcessorInfos(descriptionSb, factories.Select(static v => v.Info), "    ");
        Description = descriptionSb.ToString();
    }

    private Task<int> ExecuteAsync(ParseResult parseResult)
    {
        bool registerProcessors = parseResult.GetValue(_registerProcessorsOption);
        if (registerProcessors)
        {
            FsProcessor.Registered.Factories.UnionWith(_factories);
        }
        bool noExecuteCli = parseResult.GetValue(_noExecuteCliOption);
        if (noExecuteCli)
        {
            return Task.FromResult(0);
        }
        // respect behaviour inherited from direct parse version; args prior to double dash are inputs, args after double dash are processor arguments
        List<FpInput> fileInputs = [];
        List<string> processorArguments = [];
        bool preDoubleDash = true;
        foreach (var token in parseResult.Tokens)
        {
            switch (token.Type)
            {
                case TokenType.Argument:
                    if (preDoubleDash)
                    {
                        fileInputs.Add(ResolveFpInputForPath(token.Value));
                    }
                    else
                    {
                        processorArguments.Add(token.Value);
                    }
                    break;
                case TokenType.Command:
                    break;
                case TokenType.Option:
                    break;
                case TokenType.DoubleDash:
                    preDoubleDash = false;
                    break;
                case TokenType.Directive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        if (fileInputs.Count == 0)
        {
            Console.WriteLine("No inputs provided, exiting");
            return Task.FromResult(0);
        }
        bool preload = parseResult.GetValue(_preloadOption);
        bool debug = parseResult.GetValue(_debugOption);
        bool noop = parseResult.GetValue(_noopOption);
        int parallel = parseResult.GetValue(_multithreadOption);
        DirectoryInfo? outputRootDirectory = parseResult.GetValue(_outdirOption);
        if (outputRootDirectory == null)
        {
            string commonInput = fileInputs[0].DirectoryPath;
            outputRootDirectory = new DirectoryInfo(Path.Join(
                fileInputs.Any(input => commonInput != input.DirectoryPath || commonInput == input.Path)
                    ? Path.GetFullPath(".")
                    : commonInput,
                DefaultOutputFolderName));
        }
        string? processor = parseResult.GetValue(_processorOption);
        IReadOnlyList<FsProcessorFactory> factories = _factories;
        if (processor != null)
        {
            factories = factories.Where(v => string.Equals(v.Info.Name, processor, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (factories.Count == 0)
            {
                Console.WriteLine($"No processors matched the name [{processor}], exiting");
                return Task.FromResult(1);
            }
        }
        var configuration = new ProcessorConfiguration(processorArguments, preload, debug, noop, ConsoleLog.Default);
        var executionSettings = new FsExecutionSettings(outputRootDirectory.FullName, parallel);
        return ExecuteInternalAsync(configuration, executionSettings, fileInputs, factories);
    }

    private Task<int> ExecuteInternalAsync(
        ProcessorConfiguration processorConfiguration,
        FsExecutionSettings executionSettings,
        IReadOnlyList<FpInput> inputs,
        IReadOnlyList<FsProcessorFactory> factories)
    {
        return FsExecutor.ExecuteOnFilesystemAsync(processorConfiguration, executionSettings, inputs, _fileSystemSource, factories).ContinueWith(static _ => 0);
    }

    private static FpInput ResolveFpInputForPath(string str)
    {
        string full = Path.GetFullPath(str);
        return new FpInput(
            File.Exists(str),
            Path.GetDirectoryName(full) ?? Path.GetPathRoot(full) ?? throw new ArgumentException($"Failed to resolve path root for [{str}] (full: [{full}])"),
            full);
    }

    private static void AppendProcessorInfos(
        StringBuilder stringBuilder,
        IEnumerable<FileProcessorInfo> infos,
        string linePrefix = "")
    {
        foreach (var i in infos)
        {
            stringBuilder.Append(i.Name).AppendLine()
                .Append(linePrefix).Append("    Extensions:");
            if (i.Extensions.Length == 0) stringBuilder.Append(" <all>");
            else stringBuilder.Append(' ').Append(i.Extensions[0] ?? "<empty>");
            foreach (string? ext in i.Extensions.Skip(1))
                stringBuilder.Append(", ").Append(ext ?? "<empty>");
            stringBuilder.AppendLine()
                .Append(linePrefix).Append("    ").Append(i.ExtendedDescription.Replace("\n", "\n    ")).AppendLine();
        }
    }
}
