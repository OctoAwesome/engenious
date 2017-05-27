using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using engenious.Content.Pipeline;
using engenious.Pipeline.Pipeline.Editors;
using ContentTool.Models;

namespace ContentTool
{
    public static class PipelineHelper
    {
        private static IList<Type> _importers;
        private static readonly List<Type> Editors = new List<Type>();
        private static readonly Dictionary<string, ContentEditorWrapper> EditorsByType = new Dictionary<string, ContentEditorWrapper>();
        private static readonly Dictionary<string, Type> Processors = new Dictionary<string, Type>();


        private static readonly List<KeyValuePair<Type, string>> ProcessorsByType = new List<KeyValuePair<Type, string>>();

        private static readonly List<Assembly> Assemblies = new List<Assembly>();
        public static void DefaultInit()
        {
            Assemblies.Clear();
            Assemblies.Add(Assembly.GetExecutingAssembly());
            Assemblies.Add(typeof(IContentImporter).Assembly);
            ListImporters();
            ListProcessors();
        }
        public static string GetProcessor(string name, string importerName)
        {
            var tp = GetImporterType(Path.GetExtension(name), importerName);
            if (tp != null)
            {
                foreach (var attr in tp.GetCustomAttributes(true).Select(x => x as ContentImporterAttribute))
                {
                    if (attr == null)
                        continue;
                    return attr.DefaultProcessor;
                }
            }
            return "";
        }
        public static void PreBuilt(ContentProject currentProject)
        {
            if (currentProject == null)
                return;
            Assemblies.Clear();
            Assemblies.Add(Assembly.GetExecutingAssembly());
            Assemblies.Add(typeof(IContentImporter).Assembly);
            if (currentProject.References == null)
                currentProject.References = new List<string>();
            foreach (string reference in currentProject.References)
            {
                try
                {
                    if (File.Exists(reference))
                        Assemblies.Add(Assembly.LoadFile(reference));
                }
                catch
                {
                    // ignored
                }
            }
            ListImporters();
            ListProcessors();
            ListEditors();
        }

        private static void ListImporters()
        {
            _importers = EnumerateImporters().ToList();
        }
        public static List<string> GetProcessors(Type tp)
        {
            List<string> fitting = new List<string>();
            foreach (var pair in ProcessorsByType)
            {
                if (pair.Key.IsAssignableFrom(tp))
                {
                    fitting.Add(pair.Value);
                }
            }
            return fitting;
        }

        public static List<string> GetImporters(string extension)
        {
            List<string> fitting = new List<string>();
            foreach (var type in _importers)
            {
                var attribute =
                    (ContentImporterAttribute)type.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
                if (attribute.FileExtensions != null && attribute.FileExtensions.Contains(extension))
                    fitting.Add(attribute.DisplayName);
            }
            return fitting;
        }

        public static ContentEditorWrapper GetContentEditor(string extension, Type inputType, Type outputType)
        {
            string key = extension + "$" + inputType.FullName + "$" + outputType.FullName;
            ContentEditorWrapper editorWrap;
            if (EditorsByType.TryGetValue(key, out editorWrap))
                return editorWrap;
            Type genericType = typeof(IContentEditor<,>).MakeGenericType(inputType, outputType);

            foreach (var type in Editors)
            {
                var attribute =
                   (ContentEditorAttribute)type.GetCustomAttributes(typeof(ContentEditorAttribute), true).First();
                if (attribute == null)
                    continue;
                if (attribute.SupportedFileExtensions.Contains(extension) && genericType.IsAssignableFrom(type))
                {
                    IContentEditor editor = (IContentEditor)Activator.CreateInstance(type);
                    if (editor == null)
                        continue;
                    var methodInfo = genericType.GetMethod("Open");
                    if (methodInfo == null)
                        continue;
                    var inputOArg = Expression.Parameter(typeof (object));
                    var outputOArg = Expression.Parameter(typeof (object));


                    var openMethod = Expression.Lambda<Action<object, object>>(
                        Expression.Call(Expression.Constant(editor), methodInfo,
                            Expression.Convert(inputOArg, inputType), Expression.Convert(outputOArg, outputType)),
                        inputOArg, outputOArg).Compile();

                    editorWrap = new ContentEditorWrapper(editor,openMethod);
                    foreach (var ext in attribute.SupportedFileExtensions)
                        EditorsByType[ext + "$" + inputType.FullName + "$" + outputType.FullName] = editorWrap;
                    
                }
            }

            return editorWrap;
            //editorsByType[key] = 
        }

        private static void ListEditors()
        {
            Editors.Clear();
            foreach (var assembly in Assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IContentEditor).IsAssignableFrom(type) && !(type.IsAbstract || type.IsInterface))
                    {
                        var attribute =
                   (ContentEditorAttribute)type.GetCustomAttributes(typeof(ContentEditorAttribute), true).First();
                        if (attribute == null)
                            continue;
                        Editors.Add(type);
                    }
                }
            }
        }
        private static void ListProcessors()
        {
            Processors.Clear();
            ProcessorsByType.Clear();
            foreach (Assembly assembly in Assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {

                    if (typeof(IContentProcessor).IsAssignableFrom(type) && !(type.IsAbstract || type.IsInterface))
                    {
                        if (!Processors.ContainsKey(type.Name))
                            Processors.Add(type.Name, type);

                        Type baseType = GetProcessorInputType(type);
                        ProcessorsByType.Add(new KeyValuePair<Type, string>(baseType, type.Name));
                    }
                }
            }
        }
        public static Type GetImporterOutputType(string extension, string importerName)
        {
            var tp = GetImporterType(extension, importerName);
            var field = tp.GetField("_exportType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (field == null)
                return typeof(object);
            var lambda = Expression.Lambda<Func<Type>>(Expression.Field(null, field));
            var func = lambda.Compile();
            return func();
        }
        public static Type GetProcessorInputType(Type tp)
        {
            var field = tp.GetField("_importType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (field == null)
                return typeof(object);
            var lambda = Expression.Lambda<Func<Type>>(Expression.Field(null, field));
            var func = lambda.Compile();
            return func();
        }
        public static Type GetImporterType(string extension, string importerName)
        {
            foreach (var type in _importers)
            {
                var attribute = (ContentImporterAttribute)type.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
                if (attribute.FileExtensions.Contains(extension) && (importerName == null || attribute.DisplayName == importerName))
                    return type;
            }
            return null;
        }

        public static IContentImporter CreateImporter(string extension, ref string importerName)
        {
            if (_importers == null)
                DefaultInit();
            foreach (var type in _importers)
            {
                var attribute = (ContentImporterAttribute)type.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
                if (attribute.FileExtensions != null && attribute.FileExtensions.Contains(extension) &&
                    (string.IsNullOrEmpty(importerName) || attribute.DisplayName == importerName))
                {
                    importerName = attribute.DisplayName;
                    return (IContentImporter)Activator.CreateInstance(type);
                }
            }
            importerName = null;
            return null;
        }
        public static IContentImporter CreateImporter(string extension, string importerName = null)
        {
            return CreateImporter(extension, ref importerName);
        }

        public static IContentProcessor CreateProcessor(Type importerType, string processorName)
        {
            Type type;
            if (!string.IsNullOrEmpty(processorName) && Processors.TryGetValue(processorName, out type))
                return (IContentProcessor)Activator.CreateInstance(type);

            var attribute = (ContentImporterAttribute)importerType.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
            if (Processors.TryGetValue(attribute.DefaultProcessor, out type))
                return (IContentProcessor)Activator.CreateInstance(type);

            return null;
        }


        private static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                          typeof(TAttribute), true
                      ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        private static IEnumerable<Type> EnumerateImporters()
        {
            foreach (Assembly assembly in Assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IContentImporter).IsAssignableFrom(type))
                    {
                        if (type.IsValueType || type.IsInterface || type.IsAbstract || type.ContainsGenericParameters || type.GetConstructor(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                null, Type.EmptyTypes, null) == null)
                            continue;

                        var importerAttribute = (ContentImporterAttribute)Attribute.GetCustomAttributes(type, typeof(ContentImporterAttribute)).FirstOrDefault();
                        if (importerAttribute != null)
                        {
                            yield return type;
                        }
                    }
                }
            }
        }
    }
}

