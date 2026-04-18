using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace UnityConstantsGenerator
{
    [Generator]
    public class UnityConstantsGenerator : ISourceGenerator
    {
        private enum AnimatorParameterType
        {
            Unknown = 0,
            Float = 1,
            Int = 3,
            Bool = 4,
            Trigger = 9,
        }

        private static readonly DiagnosticDescriptor InputReadFailedDescriptor = new DiagnosticDescriptor(
            id: "UCG0002",
            title: "UnityConstant input read failed",
            messageFormat: "UnityConstant 입력 파일을 읽는 중 오류가 발생했습니다: {0}",
            category: "UnityConstantGenerator",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly HashSet<string> ReservedKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit",
            "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace", "new", "null", "object", "operator",
            "out", "override", "params", "private", "protected", "public", "readonly",
            "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc",
            "static", "string", "struct", "switch", "this", "throw", "true", "try",
            "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
            "virtual", "void", "volatile", "while"
        };

        private static readonly string[] BuiltInTags =
        {
            "Untagged",
            "Respawn",
            "Finish",
            "EditorOnly",
            "MainCamera",
            "Player",
            "GameController",
        };

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!string.Equals(context.Compilation.AssemblyName, "Assembly-CSharp", StringComparison.Ordinal))
            {
                return;
            }

            try
            {
                string sourceMethod;
                IReadOnlyDictionary<string, string> configConstants = CollectConfigConstants(context, out sourceMethod);
                IReadOnlyDictionary<string, IReadOnlyList<AnimatorParameterConstant>> animatorParametersByClass =
                    CollectAnimatorParameters(context);
                TagLayerConstantSet tagLayerConstants = CollectTagLayerConstants(context);

                string generatedSource = BuildGeneratedSource(
                    sourceMethod,
                    configConstants,
                    animatorParametersByClass,
                    tagLayerConstants);

                context.AddSource(
                    "UnityConstant_Generator.g.cs",
                    SourceText.From(generatedSource, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                context.AddSource("UnityConstant_Generator_Error.g.cs", $"/* error: {ex.Message} */");
            }
        }

        private static IReadOnlyDictionary<string, string> CollectConfigConstants(
            GeneratorExecutionContext context,
            out string sourceMethod)
        {
            string jsonText = null;
            sourceMethod = "Config source not found";

            AdditionalText configFile = context.AdditionalFiles.FirstOrDefault(
                file => file.Path.EndsWith("config.json", StringComparison.OrdinalIgnoreCase));

            if (configFile != null)
            {
                jsonText = configFile.GetText(context.CancellationToken)?.ToString();
                sourceMethod = $"AdditionalFiles: {configFile.Path}";
            }

            if (string.IsNullOrEmpty(jsonText))
            {
                foreach (string fallbackPath in EnumerateConfigPaths(context))
                {
                    try
                    {
                        if (!File.Exists(fallbackPath))
                        {
                            continue;
                        }

                        jsonText = File.ReadAllText(fallbackPath);
                        sourceMethod = $"Fallback (File.ReadAllText): {fallbackPath}";
                        break;
                    }
                    catch (Exception ex)
                    {
                        ReportReadFailure(context, fallbackPath, ex);
                    }
                }
            }

            if (string.IsNullOrEmpty(jsonText))
            {
                return new SortedDictionary<string, string>(StringComparer.Ordinal);
            }

            JObject jsonObject = JObject.Parse(jsonText);
            var constants = new SortedDictionary<string, string>(StringComparer.Ordinal);
            ParseJToken(jsonObject, string.Empty, constants);
            return constants;
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<AnimatorParameterConstant>> CollectAnimatorParameters(
            GeneratorExecutionContext context)
        {
            var controllerToParameters =
                new SortedDictionary<string, List<AnimatorParameterConstant>>(StringComparer.Ordinal);

            foreach (string controllerPath in EnumerateAnimatorControllerPaths(context))
            {
                try
                {
                    string controllerClassName = BuildAnimatorClassName(controllerPath, controllerToParameters.Keys);
                    List<AnimatorParameterConstant> parameters = ParseAnimatorParameters(
                        File.ReadAllLines(controllerPath),
                        controllerClassName);

                    controllerToParameters[controllerClassName] = parameters;
                }
                catch (Exception ex)
                {
                    ReportReadFailure(context, controllerPath, ex);
                }
            }

            return controllerToParameters.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<AnimatorParameterConstant>)pair.Value
                    .OrderBy(item => item.ParameterName, StringComparer.Ordinal)
                    .ToList(),
                StringComparer.Ordinal);
        }

        private static TagLayerConstantSet CollectTagLayerConstants(GeneratorExecutionContext context)
        {
            foreach (string tagManagerPath in EnumerateTagManagerPaths(context))
            {
                try
                {
                    if (!File.Exists(tagManagerPath))
                    {
                        continue;
                    }

                    return ParseTagManager(File.ReadAllLines(tagManagerPath));
                }
                catch (Exception ex)
                {
                    ReportReadFailure(context, tagManagerPath, ex);
                }
            }

            return TagLayerConstantSet.Empty;
        }

        private static List<AnimatorParameterConstant> ParseAnimatorParameters(
            IEnumerable<string> lines,
            string controllerClassName)
        {
            var parameters = new List<AnimatorParameterConstant>();
            var usedIdentifiers = new HashSet<string>(StringComparer.Ordinal);

            bool inParameters = false;
            string pendingParameterName = null;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (!inParameters)
                {
                    if (line.StartsWith("m_AnimatorParameters:", StringComparison.Ordinal))
                    {
                        inParameters = true;
                    }

                    continue;
                }

                if (rawLine.StartsWith("  m_AnimatorLayers:", StringComparison.Ordinal))
                {
                    break;
                }

                const string parameterNamePrefix = "- m_Name: ";
                if (line.StartsWith(parameterNamePrefix, StringComparison.Ordinal))
                {
                    pendingParameterName = line.Substring(parameterNamePrefix.Length).Trim();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(pendingParameterName))
                {
                    continue;
                }

                const string parameterTypePrefix = "m_Type: ";
                if (!line.StartsWith(parameterTypePrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                int rawType;
                if (!int.TryParse(line.Substring(parameterTypePrefix.Length).Trim(), out rawType))
                {
                    pendingParameterName = null;
                    continue;
                }

                string parameterIdentifier = MakeUniqueIdentifier(
                    SanitizeIdentifier(pendingParameterName),
                    usedIdentifiers);

                parameters.Add(new AnimatorParameterConstant(
                    controllerClassName,
                    parameterIdentifier,
                    pendingParameterName,
                    MapAnimatorParameterType(rawType)));

                pendingParameterName = null;
            }

            return parameters;
        }

        private static TagLayerConstantSet ParseTagManager(IEnumerable<string> lines)
        {
            var tags = new SortedDictionary<string, string>(StringComparer.Ordinal);
            var layers = new SortedDictionary<string, LayerConstant>(StringComparer.Ordinal);
            var usedTagIdentifiers = new HashSet<string>(StringComparer.Ordinal);
            var usedLayerIdentifiers = new HashSet<string>(StringComparer.Ordinal);
            var existingTagNames = new HashSet<string>(StringComparer.Ordinal);

            bool inTags = false;
            bool inLayers = false;
            int layerIndex = -1;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (line.Equals("tags:", StringComparison.Ordinal))
                {
                    inTags = true;
                    inLayers = false;
                    continue;
                }

                if (line.Equals("layers:", StringComparison.Ordinal))
                {
                    inTags = false;
                    inLayers = true;
                    layerIndex = -1;
                    continue;
                }

                if (line.StartsWith("m_SortingLayers:", StringComparison.Ordinal))
                {
                    break;
                }

                if (inTags && line.StartsWith("- ", StringComparison.Ordinal))
                {
                    string tagName = line.Substring(2).Trim();
                    if (string.IsNullOrWhiteSpace(tagName))
                    {
                        continue;
                    }

                    if (!existingTagNames.Add(tagName))
                    {
                        continue;
                    }

                    string tagIdentifier = MakeUniqueIdentifier(
                        SanitizeIdentifier(tagName),
                        usedTagIdentifiers);

                    tags[tagIdentifier] = tagName;
                    continue;
                }

                if (inLayers && line.StartsWith("-", StringComparison.Ordinal))
                {
                    layerIndex++;

                    string layerName = line.Length > 1 ? line.Substring(1).Trim() : string.Empty;
                    if (string.IsNullOrWhiteSpace(layerName))
                    {
                        continue;
                    }

                    if (layerIndex < 0 || layerIndex > 31)
                    {
                        continue;
                    }

                    string layerIdentifier = MakeUniqueIdentifier(
                        SanitizeIdentifier(layerName),
                        usedLayerIdentifiers);

                    layers[layerIdentifier] = new LayerConstant(layerIdentifier, layerName, layerIndex);
                }
            }

            foreach (string builtInTag in BuiltInTags)
            {
                if (!existingTagNames.Add(builtInTag))
                {
                    continue;
                }

                string tagIdentifier = MakeUniqueIdentifier(
                    SanitizeIdentifier(builtInTag),
                    usedTagIdentifiers);

                tags[tagIdentifier] = builtInTag;
            }

            return new TagLayerConstantSet(tags, layers.Values.ToList());
        }

        private static string BuildGeneratedSource(
            string sourceMethod,
            IReadOnlyDictionary<string, string> configConstants,
            IReadOnlyDictionary<string, IReadOnlyList<AnimatorParameterConstant>> animatorParametersByClass,
            TagLayerConstantSet tagLayerConstants)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine($"// Generated from: {sourceMethod}");
            sb.AppendLine("namespace UnityConstant");
            sb.AppendLine("{");

            AppendConfigClass(sb, configConstants);
            sb.AppendLine();
            AppendAnimatorClass(sb, animatorParametersByClass);
            sb.AppendLine();
            AppendTagsClass(sb, tagLayerConstants.Tags);
            sb.AppendLine();
            AppendLayersClass(sb, tagLayerConstants.Layers);

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static void AppendConfigClass(
            StringBuilder sb,
            IReadOnlyDictionary<string, string> configConstants)
        {
            sb.AppendLine("    public static partial class JsonConfig");
            sb.AppendLine("    {");

            foreach (KeyValuePair<string, string> pair in configConstants)
            {
                sb.Append("        public const string ");
                sb.Append(pair.Key);
                sb.Append(" = \"");
                sb.Append(EscapeStringLiteral(pair.Value));
                sb.AppendLine("\";");
            }

            sb.AppendLine("    }");
        }

        private static void AppendAnimatorClass(
            StringBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyList<AnimatorParameterConstant>> animatorParametersByClass)
        {
            sb.AppendLine("    public static partial class Animator");
            sb.AppendLine("    {");
            sb.AppendLine("        public static partial class Parameters");
            sb.AppendLine("        {");

            foreach (KeyValuePair<string, IReadOnlyList<AnimatorParameterConstant>> pair in animatorParametersByClass)
            {
                sb.Append("            public static partial class ");
                sb.Append(pair.Key);
                sb.AppendLine();
                sb.AppendLine("            {");

                AppendAnimatorParameterTypeClass(sb, pair.Value, AnimatorParameterType.Bool, "Bool");
                AppendAnimatorParameterTypeClass(sb, pair.Value, AnimatorParameterType.Trigger, "Trigger");
                AppendAnimatorParameterTypeClass(sb, pair.Value, AnimatorParameterType.Int, "Int");
                AppendAnimatorParameterTypeClass(sb, pair.Value, AnimatorParameterType.Float, "Float");

                sb.AppendLine("            }");
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
        }

        private static void AppendAnimatorParameterTypeClass(
            StringBuilder sb,
            IReadOnlyList<AnimatorParameterConstant> parameters,
            AnimatorParameterType parameterType,
            string typeClassName)
        {
            sb.Append("                public static partial class ");
            sb.Append(typeClassName);
            sb.AppendLine();
            sb.AppendLine("                {");

            foreach (AnimatorParameterConstant parameter in parameters)
            {
                if (parameter.ParameterType != parameterType)
                {
                    continue;
                }

                sb.Append("                    public const string ");
                sb.Append(parameter.ParameterIdentifier);
                sb.Append(" = \"");
                sb.Append(EscapeStringLiteral(parameter.ParameterName));
                sb.AppendLine("\";");
            }

            sb.AppendLine("                }");
        }

        private static void AppendTagsClass(
            StringBuilder sb,
            IReadOnlyDictionary<string, string> tags)
        {
            sb.AppendLine("    public static partial class Tags");
            sb.AppendLine("    {");

            foreach (KeyValuePair<string, string> pair in tags)
            {
                sb.Append("        public const string ");
                sb.Append(pair.Key);
                sb.Append(" = \"");
                sb.Append(EscapeStringLiteral(pair.Value));
                sb.AppendLine("\";");
            }

            sb.AppendLine("    }");
        }

        private static void AppendLayersClass(
            StringBuilder sb,
            IReadOnlyList<LayerConstant> layers)
        {
            sb.AppendLine("    public static partial class Layers");
            sb.AppendLine("    {");

            foreach (LayerConstant layer in layers)
            {
                sb.Append("        public const string ");
                sb.Append(layer.Identifier);
                sb.Append(" = \"");
                sb.Append(EscapeStringLiteral(layer.Name));
                sb.AppendLine("\";");

                sb.Append("        public const int ");
                sb.Append(layer.Identifier);
                sb.Append("Index = ");
                sb.Append(layer.Index.ToString());
                sb.AppendLine(";");

                sb.Append("        public const int ");
                sb.Append(layer.Identifier);
                sb.Append("Mask = 1 << ");
                sb.Append(layer.Identifier);
                sb.AppendLine("Index;");
            }

            sb.AppendLine("    }");
        }

        private static void ParseJToken(
            JToken token,
            string prefix,
            IDictionary<string, string> dict)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        string propertyName = SanitizeIdentifier(prop.Name);
                        string newPrefix = string.IsNullOrEmpty(prefix)
                            ? propertyName
                            : $"{prefix}_{propertyName}";

                        ParseJToken(prop.Value, newPrefix, dict);
                    }

                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken item in token.Children())
                    {
                        ParseJToken(item, $"{prefix}_{index++}", dict);
                    }

                    break;

                default:
                    if (!string.IsNullOrWhiteSpace(prefix))
                    {
                        dict[prefix] = token.ToString();
                    }

                    break;
            }
        }

        private static IEnumerable<string> EnumerateProjectRoots(GeneratorExecutionContext context)
        {
            var visitedRoots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var baseDirectories = new List<string>
            {
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory,
            };

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                "build_property.projectdir",
                out string projectDirectory))
            {
                baseDirectories.Add(projectDirectory);
            }

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                "build_property.msbuildprojectdirectory",
                out string msbuildProjectDirectory))
            {
                baseDirectories.Add(msbuildProjectDirectory);
            }

            foreach (string syntaxTreePath in context.Compilation.SyntaxTrees
                .Select(tree => tree.FilePath)
                .Where(path => !string.IsNullOrWhiteSpace(path)))
            {
                string directoryPath = Path.GetDirectoryName(syntaxTreePath);
                if (!string.IsNullOrWhiteSpace(directoryPath))
                {
                    baseDirectories.Add(directoryPath);
                }
            }

            foreach (string baseDirectory in baseDirectories.Where(path => !string.IsNullOrWhiteSpace(path)))
            {
                var current = new DirectoryInfo(baseDirectory);
                for (int depth = 0; current != null && depth < 8; depth++, current = current.Parent)
                {
                    string assetsPath = Path.Combine(current.FullName, "Assets");
                    if (!Directory.Exists(assetsPath))
                    {
                        continue;
                    }

                    if (visitedRoots.Add(current.FullName))
                    {
                        yield return current.FullName;
                    }
                }
            }
        }

        private static IEnumerable<string> EnumerateConfigPaths(GeneratorExecutionContext context)
        {
            foreach (string root in EnumerateProjectRoots(context))
            {
                yield return Path.Combine(root, "Assets", "config.json");
                yield return Path.Combine(root, "Assets", "Config.json");
            }
        }

        private static IEnumerable<string> EnumerateAnimatorControllerPaths(GeneratorExecutionContext context)
        {
            var yieldedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string root in EnumerateProjectRoots(context))
            {
                string assetsPath = Path.Combine(root, "Assets");
                if (!Directory.Exists(assetsPath))
                {
                    continue;
                }

                IEnumerable<string> controllerPaths;
                try
                {
                    controllerPaths = Directory.EnumerateFiles(
                        assetsPath,
                        "*.controller",
                        SearchOption.AllDirectories);
                }
                catch
                {
                    continue;
                }

                foreach (string controllerPath in controllerPaths.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
                {
                    if (yieldedPaths.Add(controllerPath))
                    {
                        yield return controllerPath;
                    }
                }
            }
        }

        private static IEnumerable<string> EnumerateTagManagerPaths(GeneratorExecutionContext context)
        {
            foreach (string root in EnumerateProjectRoots(context))
            {
                yield return Path.Combine(root, "ProjectSettings", "TagManager.asset");
            }
        }

        private static void ReportReadFailure(
            GeneratorExecutionContext context,
            string path,
            Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                InputReadFailedDescriptor,
                Location.None,
                $"{path} ({ex.Message})"));
        }

        private static string BuildAnimatorClassName(
            string controllerPath,
            IEnumerable<string> existingClassNames)
        {
            string fileName = Path.GetFileNameWithoutExtension(controllerPath);
            string sanitizedName = SanitizeIdentifier(fileName);
            var usedNames = new HashSet<string>(existingClassNames, StringComparer.Ordinal);

            if (!usedNames.Contains(sanitizedName))
            {
                return sanitizedName;
            }

            string[] segments = controllerPath
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToArray();

            for (int count = 2; count <= segments.Length; count++)
            {
                string suffix = string.Join("_", segments.Skip(segments.Length - count).Take(count - 1));
                string candidate = SanitizeIdentifier($"{fileName}_{suffix}");
                if (!usedNames.Contains(candidate))
                {
                    return candidate;
                }
            }

            int index = 1;
            while (true)
            {
                string candidate = $"{sanitizedName}_{index}";
                if (!usedNames.Contains(candidate))
                {
                    return candidate;
                }

                index++;
            }
        }

        private static string EscapeStringLiteral(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }

        private static string SanitizeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "_";
            }

            var sb = new StringBuilder(value.Length + 8);

            char first = value[0];
            if (!char.IsLetter(first) && first != '_')
            {
                sb.Append('_');
            }

            foreach (char ch in value)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_')
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append('_');
                }
            }

            string identifier = sb.ToString();
            if (ReservedKeywords.Contains(identifier))
            {
                identifier = "_" + identifier;
            }

            return identifier;
        }

        private static string MakeUniqueIdentifier(
            string baseIdentifier,
            ISet<string> usedIdentifiers)
        {
            string candidate = baseIdentifier;
            int index = 1;

            while (!usedIdentifiers.Add(candidate))
            {
                candidate = $"{baseIdentifier}_{index}";
                index++;
            }

            return candidate;
        }

        private static AnimatorParameterType MapAnimatorParameterType(int rawType)
        {
            switch (rawType)
            {
                case 1:
                    return AnimatorParameterType.Float;

                case 3:
                    return AnimatorParameterType.Int;

                case 4:
                    return AnimatorParameterType.Bool;

                case 9:
                    return AnimatorParameterType.Trigger;

                default:
                    return AnimatorParameterType.Unknown;
            }
        }

        private readonly struct AnimatorParameterConstant
        {
            public AnimatorParameterConstant(
                string controllerClassName,
                string parameterIdentifier,
                string parameterName,
                AnimatorParameterType parameterType)
            {
                ControllerClassName = controllerClassName;
                ParameterIdentifier = parameterIdentifier;
                ParameterName = parameterName;
                ParameterType = parameterType;
            }

            public string ControllerClassName { get; }

            public string ParameterIdentifier { get; }

            public string ParameterName { get; }

            public AnimatorParameterType ParameterType { get; }
        }

        private readonly struct TagLayerConstantSet
        {
            public static readonly TagLayerConstantSet Empty =
                new TagLayerConstantSet(
                    new SortedDictionary<string, string>(StringComparer.Ordinal),
                    new List<LayerConstant>());

            public TagLayerConstantSet(
                IReadOnlyDictionary<string, string> tags,
                IReadOnlyList<LayerConstant> layers)
            {
                Tags = tags;
                Layers = layers;
            }

            public IReadOnlyDictionary<string, string> Tags { get; }

            public IReadOnlyList<LayerConstant> Layers { get; }
        }

        private readonly struct LayerConstant
        {
            public LayerConstant(string identifier, string name, int index)
            {
                Identifier = identifier;
                Name = name;
                Index = index;
            }

            public string Identifier { get; }

            public string Name { get; }

            public int Index { get; }
        }
    }
}
