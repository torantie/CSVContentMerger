using CSVContentMerger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MergeTests
{
    [TestClass]
    public class UnitTestMerger
    {
        [TestMethod]
        public void CommaExistingFileTest()
        {
            string[] args = { "-n", "",
                "-p", @"..\..\..\..\CSV_Examples\Comma_file_exists",
                "-s", "Comma" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            var merger = new Merger(commandLineHandler);
            var gotLinesToWrite = merger.TryGetLinesToWrite(out var linesToWrite);
            foreach (var lineToWrite in linesToWrite)
            {
                var line = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), lineToWrite);
                Console.WriteLine(line);
            }
            Assert.IsTrue(linesToWrite.Count() != 0);
            Assert.IsTrue(gotLinesToWrite);
        }

        [TestMethod]
        public void CommaNewFileTest()
        {
            string[] args = { "-n", "",
                "-p", @"..\..\..\..\CSV_Examples\Comma",
                "-s", "Comma" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            var merger = new Merger(commandLineHandler);
            var gotLinesToWrite = merger.TryGetLinesToWrite(out var linesToWrite);
            foreach (var lineToWrite in linesToWrite)
            {
                var line = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), lineToWrite);
                Console.WriteLine(line);
            }
            Assert.IsTrue(linesToWrite.Count() != 0);
            Assert.IsTrue(gotLinesToWrite);
        }

        [TestMethod]
        public void SemiColonExistingFileTest()
        {
            string[] args = { "-n", "",
                "-p", @"..\..\..\..\CSV_Examples\Semi_colon_file_exists",
                "-s", "SemiColon" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            var merger = new Merger(commandLineHandler);
            var gotLinesToWrite = merger.TryGetLinesToWrite(out var linesToWrite);
            foreach (var lineToWrite in linesToWrite)
            {
                var line = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), lineToWrite);
                Console.WriteLine(line);
            }
            Assert.IsTrue(linesToWrite.Count() != 0);
            Assert.IsTrue(gotLinesToWrite);
        }

        [TestMethod]
        public void SemiColonNewFileTest()
        {
            string[] args = { "-n", "",
                "-p", @"..\..\..\..\CSV_Examples\Semi_colon",
                "-s", "SemiColon" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            var merger = new Merger(commandLineHandler);
            var gotLinesToWrite = merger.TryGetLinesToWrite(out var linesToWrite);
            foreach (var lineToWrite in linesToWrite)
            {
                var line = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), lineToWrite);
                Console.WriteLine(line);
            }
            Assert.IsTrue(linesToWrite.Count() != 0);
            Assert.IsTrue(gotLinesToWrite);
        }

        [TestMethod]
        public void NewLineSpaceExistingFileTest()
        {
            string[] args = { "-n", "",
                "-p", @"..\..\..\..\CSV_Examples\Line_breaks_file_exists",
                "-s", "NewLineSpace" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            var merger = new Merger(commandLineHandler);
            var gotLinesToWrite = merger.TryGetLinesToWrite(out var linesToWrite);
            foreach (var lineToWrite in linesToWrite)
            {
                var line = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), lineToWrite);
                Console.WriteLine(line);
            }
            Assert.IsTrue(linesToWrite.Count() != 0);
            Assert.IsTrue(gotLinesToWrite);
        }

        [TestMethod]
        public void NewLineSpaceNewFileTest()
        {
            string[] args = { "-n", "",
                "-p", @"..\..\..\..\CSV_Examples\Line_breaks",
                "-s", "NewLineSpace" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            var merger = new Merger(commandLineHandler);
            var gotLinesToWrite = merger.TryGetLinesToWrite(out var linesToWrite);
            foreach (var lineToWrite in linesToWrite)
            {
                var line = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), lineToWrite);
                Console.WriteLine(line);
            }
            Assert.IsTrue(linesToWrite.Count() != 0);
            Assert.IsTrue(gotLinesToWrite);
        }
    }
}
