using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CSVContentMerger
{
    public class CommandLineHandler
    {
        public string MergeFileName { get; set; } = DefaultArgumentValues.DefaultMergeFileName;

        public string MergeFileDirectoryPath { get; set; } = DefaultArgumentValues.DefaultMergeFileDirectoryPath;

        public string MergeFilePath { get; set; } = "";

        public SeparatorType SeparatorType { get; set; } = DefaultArgumentValues.DefaultSeparatorType;

        public void HandleProgramStart(string[] a_args)
        {
            PrintStartMessages();
            ParseArguments(a_args);
        }

        public void HandleProgramEnd()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Finished...");
            Console.WriteLine("-----------");
        }

        private void PrintStartMessages()
        {
            Console.WriteLine(string.Format("Default csv merge file name <{0}>", DefaultArgumentValues.DefaultMergeFileName));
            Console.WriteLine(string.Format("Default merge file directory path <{0}>", DefaultArgumentValues.DefaultMergeFileDirectoryPath));
            var separatorTypes = Enum.GetValues(typeof(SeparatorType)).Cast<SeparatorType>();
            var separatorTypeStringCollection = separatorTypes.Select(separatorType => string.Format("<{0}>", separatorType.ToString()));
            var separatorTypesString = string.Join(',', separatorTypeStringCollection);

            Console.WriteLine(string.Format("Default separator type <{0}>. Option: {1}",
                DefaultArgumentValues.DefaultSeparatorType,
                separatorTypesString));
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Type --help to see the command names.");
        }

        private void ParseArguments(string[] a_args)
        {
            while (true)
            {
                var args = a_args;
                if (args.Length == 0)
                {
                    var defaultCommand = string.Format("-n {0} -p {1} -s {2}", MergeFileName, MergeFileDirectoryPath, SeparatorType);
                    args = CommandLineHandler.CommandLineToArgs(defaultCommand);
                }

                var parseResult = Parser.Default.ParseArguments<Options>(args).WithParsed(o => HandleParsedArguments(o));

                if (parseResult.Tag != ParserResultType.NotParsed)
                    break;
            }
        }

        private void HandleParsedArguments(Options a_o)
        {
            if (!string.IsNullOrEmpty(a_o.MergeFileName))
            {
                MergeFileName = a_o.MergeFileName;
            }
            if (!string.IsNullOrEmpty(a_o.MergeFileDirectoryPath))
            {
                MergeFileDirectoryPath = a_o.MergeFileDirectoryPath;
            }
            if (!string.IsNullOrEmpty(a_o.SeparatorType))
            {
                if (Enum.TryParse(a_o.SeparatorType, true, out SeparatorType separatorType))
                    SeparatorType = separatorType;
                else
                    SeparatorType = SeparatorType.NewLineSpace;
            }

            Console.WriteLine(string.Format("Executing command: -n {0} -p {1} -s {2}", MergeFileName, MergeFileDirectoryPath, SeparatorType));

            MergeFilePath = Path.Combine(MergeFileDirectoryPath, MergeFileName);
        }

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        /// <summary>
        /// https://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp
        /// </summary>
        /// <param name="a_commandLine"></param>
        /// <returns></returns>
        public static string[] CommandLineToArgs(string a_commandLine)
        {
            var argv = CommandLineToArgvW(a_commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }
}
