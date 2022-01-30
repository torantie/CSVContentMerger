using CSVContentMerger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MergeTests
{
    [TestClass]
    public class UnitTestCommandLineHandler
    {
        [TestMethod]
        public void TestMethod1()
        {
            var separatorType = Enum.GetName(typeof(SeparatorType), DefaultArgumentValues.DefaultSeparatorType);
            string[] args = { "-n", DefaultArgumentValues.DefaultMergeFileName,
                "-p", DefaultArgumentValues.DefaultMergeFileDirectoryPath,
                "-s",  separatorType};
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            Assert.AreEqual(commandLineHandler.MergeFileName, DefaultArgumentValues.DefaultMergeFileName);
            Assert.AreEqual(commandLineHandler.MergeFileDirectoryPath, DefaultArgumentValues.DefaultMergeFileDirectoryPath);
            Assert.AreEqual(commandLineHandler.SeparatorType, DefaultArgumentValues.DefaultSeparatorType);
        }

        [TestMethod]
        public void TestMethod2()
        {
            string[] args = { "-n", "",
                "-p", "",
                "-s", "" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            Assert.AreEqual(commandLineHandler.MergeFileName, DefaultArgumentValues.DefaultMergeFileName);
            Assert.AreEqual(commandLineHandler.MergeFileDirectoryPath, DefaultArgumentValues.DefaultMergeFileDirectoryPath);
            Assert.AreEqual(commandLineHandler.SeparatorType, DefaultArgumentValues.DefaultSeparatorType);
        }

        [TestMethod]
        public void TestMethod3()
        {
            string[] args = { "-n", "",
                "-p", "",
                "-s", "Comma" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            Assert.AreEqual(commandLineHandler.MergeFileName, DefaultArgumentValues.DefaultMergeFileName);
            Assert.AreEqual(commandLineHandler.MergeFileDirectoryPath, DefaultArgumentValues.DefaultMergeFileDirectoryPath);
            Assert.AreEqual(commandLineHandler.SeparatorType, SeparatorType.Comma);
        }

        [TestMethod]
        public void TestMethod4()
        {
            string[] args = { "-n", "",
                "-p", "",
                "-s", "SemiColon" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            Assert.AreEqual(commandLineHandler.MergeFileName, DefaultArgumentValues.DefaultMergeFileName);
            Assert.AreEqual(commandLineHandler.MergeFileDirectoryPath, DefaultArgumentValues.DefaultMergeFileDirectoryPath);
            Assert.AreEqual(commandLineHandler.SeparatorType, SeparatorType.SemiColon);
        }

        [TestMethod]
        public void TestMethod5()
        {
            string[] args = { "-n", "",
                "-p", "",
                "-s", "NewLineSpace" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            Assert.AreEqual(commandLineHandler.MergeFileName, DefaultArgumentValues.DefaultMergeFileName);
            Assert.AreEqual(commandLineHandler.MergeFileDirectoryPath, DefaultArgumentValues.DefaultMergeFileDirectoryPath);
            Assert.AreEqual(commandLineHandler.SeparatorType, SeparatorType.NewLineSpace);
        }

        [TestMethod]
        public void TestMethod6()
        {
            var testName = "TestName";
            string[] args = { "-n", testName,
                "-p", "",
                "-s", "" };
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.HandleProgramStart(args);
            Assert.AreEqual(commandLineHandler.MergeFileName, testName);
            Assert.AreEqual(commandLineHandler.MergeFileDirectoryPath, DefaultArgumentValues.DefaultMergeFileDirectoryPath);
            Assert.AreEqual(commandLineHandler.SeparatorType, DefaultArgumentValues.DefaultSeparatorType);
        }
    }
}
